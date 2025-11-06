using System.ComponentModel.DataAnnotations;

namespace CrudNN.Web.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "O nome de usuário é obrigatório")]
    [Display(Name = "Nome de usuário")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Lembrar-me")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
