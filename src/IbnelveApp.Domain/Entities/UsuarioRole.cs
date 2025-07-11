using System;
using System.Data;

namespace IbnelveApp.Domain.Entities
{
    public class UsuarioRole
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public Guid RoleId { get; set; }
        public Role Role { get; set; }

        // (Opcional) Data de atribuição do papel
        public DateTime AtribuidoEm { get; set; } = DateTime.UtcNow;
    }
}