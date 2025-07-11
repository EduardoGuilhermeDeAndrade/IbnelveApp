using IbnelveApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IbnelveApp.Infrastructure.Mappings;

public class EquipamentoMap : IEntityTypeConfiguration<Equipamento>
{
    public void Configure(EntityTypeBuilder<Equipamento> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Nome).IsRequired().HasMaxLength(100);
        builder.Property(e => e.NumeroControle).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Observacoes).HasMaxLength(500);
        builder.Property(e => e.DataCriacao).IsRequired();
        builder.Property(e => e.Status).IsRequired();
    }
}
