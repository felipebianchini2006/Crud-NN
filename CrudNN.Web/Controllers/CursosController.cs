using CrudNN.Web.Data;
using CrudNN.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrudNN.Web.Controllers;

[Authorize]
public class CursosController(ApplicationDbContext context) : Controller
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IActionResult> Index(string? search)
    {
        var query = _context.Cursos.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(c => EF.Functions.ILike(c.Titulo, $"%{term}%")
                                 || (c.Descricao != null && EF.Functions.ILike(c.Descricao, $"%{term}%")));
        }

        var cursos = await query.OrderBy(c => c.Titulo).ToListAsync();
        ViewData["Search"] = search;
        return View(cursos);
    }

    public IActionResult Create()
    {
        return View(new Curso());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Curso curso)
    {
        if (!ModelState.IsValid)
        {
            return View(curso);
        }

        _context.Add(curso);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Curso criado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var curso = await _context.Cursos
            .Include(c => c.Matriculas)
            .ThenInclude(m => m.Aluno)
            .FirstOrDefaultAsync(c => c.Id == id);

        return curso is null ? NotFound() : View(curso);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var curso = await _context.Cursos.FindAsync(id);
        return curso is null ? NotFound() : View(curso);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Curso curso)
    {
        if (id != curso.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(curso);
        }

        try
        {
            _context.Update(curso);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Curso atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await CursoExists(curso.Id))
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

        var curso = await _context.Cursos
            .Include(c => c.Matriculas)
            .ThenInclude(m => m.Aluno)
            .FirstOrDefaultAsync(c => c.Id == id);

        return curso is null ? NotFound() : View(curso);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var curso = await _context.Cursos.FindAsync(id);
        if (curso is null)
        {
            return NotFound();
        }

        if (await _context.Matriculas.AnyAsync(m => m.CursoId == id))
        {
            TempData["Error"] = "N�o � poss�vel excluir o curso porque h� matr�culas associadas.";
            return RedirectToAction(nameof(Delete), new { id });
        }

        _context.Cursos.Remove(curso);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Curso removido com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> CursoExists(int id) => await _context.Cursos.AnyAsync(e => e.Id == id);
}
