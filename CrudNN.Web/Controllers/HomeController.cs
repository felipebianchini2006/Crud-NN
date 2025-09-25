using System.Diagnostics;
using CrudNN.Web.Data;
using CrudNN.Web.Models;
using CrudNN.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrudNN.Web.Controllers;

public class HomeController(ILogger<HomeController> logger, ApplicationDbContext context) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    private readonly ApplicationDbContext _context = context;

    public async Task<IActionResult> Index()
    {
        var totalAlunos = await _context.Alunos.CountAsync();
        var totalCursos = await _context.Cursos.CountAsync();
        var matriculasAtivas = await _context.Matriculas.CountAsync(m => m.Status == MatriculaStatus.Ativo);
        var matriculasConcluidas = await _context.Matriculas.CountAsync(m => m.Status == MatriculaStatus.Concluido);

        var ultimas = await _context.Matriculas
            .Include(m => m.Aluno)
            .Include(m => m.Curso)
            .OrderByDescending(m => m.Data)
            .Take(5)
            .Select(m => new DashboardMatriculaResumo
            {
                Aluno = m.Aluno.Nome,
                Curso = m.Curso.Titulo,
                Data = m.Data,
                Status = m.Status,
                Progresso = m.Progresso
            })
            .ToListAsync();

        var viewModel = new DashboardViewModel
        {
            TotalAlunos = totalAlunos,
            TotalCursos = totalCursos,
            MatriculasAtivas = matriculasAtivas,
            MatriculasConcluidas = matriculasConcluidas,
            UltimasMatriculas = ultimas
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
