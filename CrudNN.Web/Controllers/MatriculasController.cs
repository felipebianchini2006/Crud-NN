using CrudNN.Web.Data;
using CrudNN.Web.Models;
using CrudNN.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CrudNN.Web.Controllers;

public class MatriculasController(ApplicationDbContext context) : Controller
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IActionResult> Index(string? search, MatriculaStatus? status)
    {
        var query = _context.Matriculas
            .Include(m => m.Aluno)
            .Include(m => m.Curso)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(m => EF.Functions.ILike(m.Aluno.Nome, $"%{term}%")
                                 || EF.Functions.ILike(m.Curso.Titulo, $"%{term}%"));
        }

        if (status.HasValue)
        {
            query = query.Where(m => m.Status == status);
        }

        var matriculas = await query
            .OrderBy(m => m.Aluno.Nome)
            .ThenBy(m => m.Curso.Titulo)
            .ToListAsync();

        ViewData["Search"] = search;
        ViewData["Status"] = status;
        return View(matriculas);
    }

    public async Task<IActionResult> Details(int? alunoId, int? cursoId)
    {
        if (alunoId is null || cursoId is null)
        {
            return NotFound();
        }

        var matricula = await _context.Matriculas
            .Include(m => m.Aluno)
            .Include(m => m.Curso)
            .FirstOrDefaultAsync(m => m.AlunoId == alunoId && m.CursoId == cursoId);

        return matricula is null ? NotFound() : View(matricula);
    }

    public async Task<IActionResult> Manage(int? alunoId)
    {
        await PopulateAlunosSelectListAsync(alunoId);

        if (alunoId is null)
        {
            return View(new MatriculaGerenciarViewModel());
        }

        var viewModel = await BuildManageViewModelAsync(alunoId.Value, null);
        if (viewModel is null)
        {
            TempData["Error"] = "Aluno não encontrado.";
            return RedirectToAction(nameof(Manage));
        }

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Manage(MatriculaGerenciarViewModel viewModel)
    {
        await PopulateAlunosSelectListAsync(viewModel.AlunoId);

        var hydrated = await BuildManageViewModelAsync(viewModel.AlunoId, viewModel.Cursos);
        if (hydrated is null)
        {
            ModelState.AddModelError(nameof(MatriculaGerenciarViewModel.AlunoId), "Aluno não encontrado.");
            return View(new MatriculaGerenciarViewModel());
        }

        viewModel = hydrated;

        var selected = viewModel.Cursos.Where(c => c.Selecionado).ToList();
        if (selected.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Selecione ao menos um curso.");
        }

        for (var i = 0; i < viewModel.Cursos.Count; i++)
        {
            var item = viewModel.Cursos[i];
            if (!item.Selecionado)
            {
                continue;
            }

            if (item.PrecoPago is null)
            {
                ModelState.AddModelError($"Cursos[{i}].PrecoPago", "Informe o preço pago.");
            }

            if (item.Progresso is null)
            {
                ModelState.AddModelError($"Cursos[{i}].Progresso", "Informe o progresso.");
            }
            else if (item.Status == MatriculaStatus.Concluido && item.Progresso < 100)
            {
                ModelState.AddModelError($"Cursos[{i}].Progresso", "Para concluir o curso o progresso deve ser 100.");
            }
        }

        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var existingMatriculas = await _context.Matriculas
            .Where(m => m.AlunoId == viewModel.AlunoId)
            .ToListAsync();

        foreach (var item in viewModel.Cursos)
        {
            var matricula = existingMatriculas.FirstOrDefault(m => m.CursoId == item.CursoId);
            if (item.Selecionado)
            {
                if (matricula is null)
                {
                    matricula = new Matricula
                    {
                        AlunoId = viewModel.AlunoId,
                        CursoId = item.CursoId,
                        Data = DateTime.UtcNow
                    };
                    _context.Matriculas.Add(matricula);
                }

                matricula.PrecoPago = item.PrecoPago!.Value;
                matricula.Progresso = item.Progresso ?? 0;
                matricula.NotaFinal = item.NotaFinal;
                matricula.Status = item.Status;
            }
            else if (matricula is not null)
            {
                _context.Matriculas.Remove(matricula);
            }
        }

        await _context.SaveChangesAsync();
        TempData["Success"] = "Matrículas atualizadas com sucesso.";
        return RedirectToAction(nameof(Manage), new { alunoId = viewModel.AlunoId });
    }

    public async Task<IActionResult> Delete(int? alunoId, int? cursoId)
    {
        if (alunoId is null || cursoId is null)
        {
            return NotFound();
        }

        var matricula = await _context.Matriculas
            .Include(m => m.Aluno)
            .Include(m => m.Curso)
            .FirstOrDefaultAsync(m => m.AlunoId == alunoId && m.CursoId == cursoId);

        return matricula is null ? NotFound() : View(matricula);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int alunoId, int cursoId)
    {
        var matricula = await _context.Matriculas.FindAsync(alunoId, cursoId);
        if (matricula is null)
        {
            return NotFound();
        }

        _context.Matriculas.Remove(matricula);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Matrícula removida com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateAlunosSelectListAsync(int? selectedId)
    {
        var alunos = await _context.Alunos.OrderBy(a => a.Nome).ToListAsync();
        ViewBag.Alunos = new SelectList(alunos, nameof(Aluno.Id), nameof(Aluno.Nome), selectedId);
    }

    private async Task<MatriculaGerenciarViewModel?> BuildManageViewModelAsync(int alunoId, IEnumerable<MatriculaCursoItemViewModel>? postedCursos)
    {
        var aluno = await _context.Alunos
            .Include(a => a.Matriculas)
            .ThenInclude(m => m.Curso)
            .FirstOrDefaultAsync(a => a.Id == alunoId);

        if (aluno is null)
        {
            return null;
        }

        var cursos = await _context.Cursos.OrderBy(c => c.Titulo).ToListAsync();
        var posted = postedCursos?.ToDictionary(c => c.CursoId);

        var itens = new List<MatriculaCursoItemViewModel>();
        foreach (var curso in cursos)
        {
            var existing = aluno.Matriculas.FirstOrDefault(m => m.CursoId == curso.Id);
            if (posted is not null && posted.TryGetValue(curso.Id, out var itemFromPost))
            {
                itemFromPost.Titulo = curso.Titulo;
                itemFromPost.PrecoBase = curso.PrecoBase;
                itemFromPost.Data = existing?.Data;
                itens.Add(itemFromPost);
                continue;
            }

            itens.Add(new MatriculaCursoItemViewModel
            {
                CursoId = curso.Id,
                Titulo = curso.Titulo,
                PrecoBase = curso.PrecoBase,
                Selecionado = existing is not null,
                PrecoPago = existing?.PrecoPago ?? curso.PrecoBase,
                Progresso = existing?.Progresso ?? 0,
                NotaFinal = existing?.NotaFinal,
                Status = existing?.Status ?? MatriculaStatus.Ativo,
                Data = existing?.Data
            });
        }

        return new MatriculaGerenciarViewModel
        {
            AlunoId = alunoId,
            AlunoNome = aluno.Nome,
            Cursos = itens
        };
    }
}
