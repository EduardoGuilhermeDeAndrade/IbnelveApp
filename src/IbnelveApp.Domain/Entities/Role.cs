using System;
using System.Collections.Generic;

namespace IbnelveApp.Domain.Entities
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }

        public ICollection<UsuarioRole> UsuarioRoles { get; set; } = new List<UsuarioRole>();
    }
}
