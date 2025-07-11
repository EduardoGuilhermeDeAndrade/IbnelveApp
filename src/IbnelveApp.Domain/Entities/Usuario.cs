using System.Text.Json.Serialization;

namespace IbnelveApp.Domain.Entities
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        [JsonIgnore] // Essencial para não expor o hash
        public string SenhaHash { get; set; }
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public ICollection<UsuarioRole> UsuarioRoles { get; set; } = new List<UsuarioRole>();
    }
}
