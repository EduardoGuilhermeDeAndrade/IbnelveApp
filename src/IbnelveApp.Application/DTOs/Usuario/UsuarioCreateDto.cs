using System.ComponentModel.DataAnnotations;

namespace IbnelveApp.Application.DTOs.Usuario;

public class UsuarioCreateDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "O formato do email é inválido.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "A senha deve ter no mínimo 8 caracteres.")]
    public string Senha { get; set; }

    [Required(ErrorMessage = "O TenantId é obrigatório.")]
    public Guid TenantId { get; set; }

    // Opcional: Receber as roles no momento da criação
    public IEnumerable<Guid> RoleIds { get; set; } = new List<Guid>();
}
