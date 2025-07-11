using IbnelveApp.Domain.Entities;

namespace IbnelveApp.Application.Interfaces;

public interface IEquipamentoRepositorio : IRepositorioBase<Equipamento>
{
    // Aqui você pode adicionar métodos específicos, se quiser, ex:
    Task<Equipamento?> ObterPorNumeroControleAsync(string numeroControle);
}
