using System.ComponentModel.DataAnnotations;

namespace IbnelveApp.Application.DTOs.Auth;

public class LoginDto
{
    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "O formato do email é inválido.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string Senha { get; set; }
}