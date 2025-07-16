namespace IbnelveApp.Application.DTOs.Usuario;

public class UsuarioDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public Guid TenantId { get; set; }
    public IEnumerable<string> Roles { get; set; } // Lista de nomes das roles para simplicidade
}
