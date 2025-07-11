using IbnelveApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IbnelveApp.Infrastructure.Data.Configurations
{
    public class UsuarioRoleConfiguration : IEntityTypeConfiguration<UsuarioRole>
    {
        public void Configure(EntityTypeBuilder<UsuarioRole> builder)
        {
            // Chave composta
            builder.HasKey(ur => new { ur.UsuarioId, ur.RoleId });

            // Relacionamento com Usuario
            builder.HasOne(ur => ur.Usuario)
                   .WithMany(u => u.UsuarioRoles)
                   .HasForeignKey(ur => ur.UsuarioId);

            // Relacionamento com Role
            builder.HasOne(ur => ur.Role)
                   .WithMany(r => r.UsuarioRoles)
                   .HasForeignKey(ur => ur.RoleId);

            // Nome da tabela no banco
            builder.ToTable("UsuarioRoles");
        }
    }
}
