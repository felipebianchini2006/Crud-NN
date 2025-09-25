using CrudNN.Web.Models;

namespace CrudNN.Web.ViewModels;

public class DashboardViewModel
{
    public int TotalAlunos { get; set; }
    public int TotalCursos { get; set; }
    public int MatriculasAtivas { get; set; }
    public int MatriculasConcluidas { get; set; }
    public List<DashboardMatriculaResumo> UltimasMatriculas { get; set; } = new();
}

public class DashboardMatriculaResumo
{
    public required string Aluno { get; set; }
    public required string Curso { get; set; }
    public DateTime Data { get; set; }
    public MatriculaStatus Status { get; set; }
    public int Progresso { get; set; }
}
