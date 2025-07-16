using System.ComponentModel.DataAnnotations;

namespace IbnelveApp.Application.DTOs.Usuario;

public class UsuarioUpdateDto
{
    [Required]
    public Guid Id { get; set; } // Id para identificar o usuário a ser atualizado

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "O formato do email é inválido.")]
    public string Email { get; set; }

    // As roles podem ser atualizadas aqui
    [Required]
    public IEnumerable<Guid> RoleIds { get; set; }
}
