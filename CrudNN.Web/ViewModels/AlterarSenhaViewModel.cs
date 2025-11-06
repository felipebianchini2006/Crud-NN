using System.ComponentModel.DataAnnotations;

namespace CrudNN.Web.ViewModels;

public class AlterarSenhaViewModel
{
    [Required(ErrorMessage = "A senha atual é obrigatória")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha atual")]
    public string SenhaAtual { get; set; } = string.Empty;

    [Required(ErrorMessage = "A nova senha é obrigatória")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
    [DataType(DataType.Password)]
    [Display(Name = "Nova senha")]
    public string NovaSenha { get; set; } = string.Empty;

    [Required(ErrorMessage = "A confirmação da senha é obrigatória")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar nova senha")]
    [Compare("NovaSenha", ErrorMessage = "A senha e a confirmação não coincidem")]
    public string ConfirmarSenha { get; set; } = string.Empty;
}
