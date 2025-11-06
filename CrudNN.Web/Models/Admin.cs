using System.ComponentModel.DataAnnotations;

namespace CrudNN.Web.Models;

public class Admin
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome de usuário é obrigatório")]
    [StringLength(50, ErrorMessage = "O nome de usuário deve ter no máximo 50 caracteres")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome completo é obrigatório")]
    [StringLength(100, ErrorMessage = "O nome completo deve ter no máximo 100 caracteres")]
    public string NomeCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória")]
    public string PasswordHash { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public DateTime? UltimoAcesso { get; set; }
}
