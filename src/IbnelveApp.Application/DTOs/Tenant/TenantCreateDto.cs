using System.ComponentModel.DataAnnotations;

namespace IbnelveApp.Application.DTOs.Tenant;

public class TenantCreateDto
{
    [Required(ErrorMessage = "O nome do tenant é obrigatório.")]
    [StringLength(100)]
    public string Nome { get; set; }
}
