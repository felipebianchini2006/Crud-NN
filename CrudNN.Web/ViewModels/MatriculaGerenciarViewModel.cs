using System.ComponentModel.DataAnnotations;

namespace CrudNN.Web.ViewModels;

public class MatriculaGerenciarViewModel
{
    [Display(Name = "Aluno")]
    [Required(ErrorMessage = "Selecione um aluno.")]
    public int AlunoId { get; set; }

    public string AlunoNome { get; set; } = string.Empty;

    public List<MatriculaCursoItemViewModel> Cursos { get; set; } = new();
}
