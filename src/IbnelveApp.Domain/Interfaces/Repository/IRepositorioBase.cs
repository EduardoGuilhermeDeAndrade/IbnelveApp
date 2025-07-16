﻿namespace IbnelveApp.Application.Interfaces.Repositorios;

public interface IRepositorioBase<T> where T : class
{
    Task<T?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<T>> ObterTodosAsync();
    Task AdicionarAsync(T entidade);
    Task AtualizarAsync(T entidade);
    Task RemoverAsync(T entidade);
}
