using System.ComponentModel.DataAnnotations;
using CrudNN.Web.Models;

namespace CrudNN.Web.ViewModels;

public class MatriculaCursoItemViewModel
{
    public int CursoId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public bool Selecionado { get; set; }

    [Display(Name = "Preço pago")]
    [DataType(DataType.Currency)]
    [Range(0, double.MaxValue, ErrorMessage = "O preço pago deve ser maior ou igual a zero.")]
    public decimal? PrecoPago { get; set; }

    [Range(0, 100, ErrorMessage = "O progresso deve estar entre 0 e 100.")]
    public int? Progresso { get; set; }

    [Display(Name = "Nota final")]
    [Range(typeof(decimal), "0", "10", ErrorMessage = "A nota final deve estar entre 0 e 10.")]
    public decimal? NotaFinal { get; set; }

    public MatriculaStatus Status { get; set; } = MatriculaStatus.Ativo;

    public decimal PrecoBase { get; set; }
    public DateTime? Data { get; set; }
}
