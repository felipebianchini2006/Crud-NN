using System.ComponentModel.DataAnnotations;

namespace CrudNN.Web.Models;

public class Aluno
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(150)]
    public string Nome { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [Display(Name = "E-mail")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Telefone inválido.")]
    [Display(Name = "Telefone")]
    public string? Telefone { get; set; }

    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}
