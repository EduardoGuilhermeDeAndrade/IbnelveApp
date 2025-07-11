using IbnelveApp.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using IbnelveApp.Infrastructure.Data;

namespace IbnelveApp.Infrastructure.Repositories;

public class RepositorioBase<T> : IRepositorioBase<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public RepositorioBase(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> ObterPorIdAsync(Guid id) =>
        await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> ObterTodosAsync() =>
        await _dbSet.ToListAsync();

    public async Task AdicionarAsync(T entidade)
    {
        await _dbSet.AddAsync(entidade);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(T entidade)
    {
        _context.Entry(entidade).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(T entidade)
    {
        _dbSet.Remove(entidade);
        await _context.SaveChangesAsync();
    }
}
