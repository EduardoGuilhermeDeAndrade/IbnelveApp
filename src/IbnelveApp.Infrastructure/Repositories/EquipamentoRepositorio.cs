using IbnelveApp.Application.Interfaces.Repositorios;
using IbnelveApp.Domain.Entities;
using IbnelveApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IbnelveApp.Infrastructure.Repositories;

public class EquipamentoRepositorio : RepositorioBase<Equipamento>, IEquipamentoRepositorio
{
    public EquipamentoRepositorio(AppDbContext context) : base(context) { }

    public async Task<Equipamento?> ObterPorNumeroControleAsync(string numeroControle)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.NumeroControle == numeroControle);
    }
}
