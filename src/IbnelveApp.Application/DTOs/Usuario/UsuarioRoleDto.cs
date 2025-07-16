using System.ComponentModel.DataAnnotations;

namespace IbnelveApp.Application.DTOs.Usuario;

public class UsuarioRoleDto
{
    [Required]
    public Guid UsuarioId { get; set; }

    [Required]
    public Guid RoleId { get; set; }
}
