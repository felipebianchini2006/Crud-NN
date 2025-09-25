using CrudNN.Web.Data;
using CrudNN.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrudNN.Web.Controllers;

public class AlunosController(ApplicationDbContext context) : Controller
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IActionResult> Index(string? search)
    {
        var query = _context.Alunos.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(a => EF.Functions.ILike(a.Nome, $"%{term}%")
                                 || (a.Email != null && EF.Functions.ILike(a.Email, $"%{term}%"))
                                 || (a.Telefone != null && EF.Functions.ILike(a.Telefone, $"%{term}%")));
        }

        var alunos = await query.OrderBy(a => a.Nome).ToListAsync();
        ViewData["Search"] = search;
        return View(alunos);
    }

    public IActionResult Create()
    {
        return View(new Aluno());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Aluno aluno)
    {
        if (!ModelState.IsValid)
        {
            return View(aluno);
        }

        _context.Add(aluno);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Aluno criado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var aluno = await _context.Alunos
            .Include(a => a.Matriculas)
            .ThenInclude(m => m.Curso)
            .FirstOrDefaultAsync(a => a.Id == id);

        return aluno is null ? NotFound() : View(aluno);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var aluno = await _context.Alunos.FindAsync(id);
        return aluno is null ? NotFound() : View(aluno);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Aluno aluno)
    {
        if (id != aluno.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(aluno);
        }

        try
        {
            _context.Update(aluno);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Aluno atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await AlunoExists(aluno.Id))
            {
                return NotFound();
            }
            throw;
        }
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var aluno = await _context.Alunos
            .Include(a => a.Matriculas)
            .ThenInclude(m => m.Curso)
            .FirstOrDefaultAsync(a => a.Id == id);

        return aluno is null ? NotFound() : View(aluno);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var aluno = await _context.Alunos.FindAsync(id);
        if (aluno is null)
        {
            return NotFound();
        }

        if (await _context.Matriculas.AnyAsync(m => m.AlunoId == id))
        {
            TempData["Error"] = "Não é possível excluir o aluno porque há matrículas associadas.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        _context.Alunos.Remove(aluno);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Aluno removido com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> AlunoExists(int id) => await _context.Alunos.AnyAsync(e => e.Id == id);
}
