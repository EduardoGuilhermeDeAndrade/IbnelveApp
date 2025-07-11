using IbnelveApp.Application.DTOs;

namespace IbnelveApp.Application.Interfaces;

public interface IEquipamentoService
{
    Task<IEnumerable<EquipamentoDto>> ObterTodosAsync();
    Task<EquipamentoDto> ObterPorNumeroControleAsync(string numeroControle);
    Task<EquipamentoDto> ObterPorIdAsync(Guid id);
    Task AdicionarAsync(EquipamentoDto dto);
    Task AtualizarAsync(EquipamentoDto dto);
    Task RemoverAsync(Guid id, bool logico = true);
}
