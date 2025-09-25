using System.ComponentModel.DataAnnotations;

namespace CrudNN.Web.Models;

public class Matricula
{
    public int AlunoId { get; set; }
    public Aluno Aluno { get; set; } = null!;

    public int CursoId { get; set; }
    public Curso Curso { get; set; } = null!;

    [Display(Name = "Data da matrícula")]
    [DataType(DataType.Date)]
    public DateTime Data { get; set; } = DateTime.UtcNow;

    [Range(0, double.MaxValue, ErrorMessage = "O preço pago deve ser maior ou igual a zero.")]
    [Display(Name = "Preço pago")]
    [DataType(DataType.Currency)]
    public decimal PrecoPago { get; set; }

    [Display(Name = "Status")]
    public MatriculaStatus Status { get; set; } = MatriculaStatus.Ativo;

    [Range(0, 100, ErrorMessage = "O progresso deve estar entre 0 e 100.")]
    public int Progresso { get; set; }

    [Display(Name = "Nota final")]
    [Range(typeof(decimal), "0", "10", ErrorMessage = "A nota final deve estar entre 0 e 10.")]
    public decimal? NotaFinal { get; set; }
}
