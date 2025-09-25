using System.ComponentModel.DataAnnotations;

namespace CrudNN.Web.Models;

public class Curso
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(150)]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [Display(Name = "Descrição")]
    public string? Descricao { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "O preço base deve ser maior ou igual a zero.")]
    [Display(Name = "Preço base")]
    public decimal PrecoBase { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "A carga horária deve ser maior que zero.")]
    [Display(Name = "Carga horária (h)")]
    public int CargaHoraria { get; set; }

    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}
