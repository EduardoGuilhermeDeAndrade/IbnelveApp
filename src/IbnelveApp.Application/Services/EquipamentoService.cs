using IbnelveApp.Application.DTOs;
using IbnelveApp.Application.Interfaces;
using IbnelveApp.Application.Mappings;
using IbnelveApp.Domain.Entities;

namespace IbnelveApp.Application.Services;

public class EquipamentoService : IEquipamentoService
{
    private readonly IEquipamentoRepositorio _repositorio;

    public EquipamentoService(IEquipamentoRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IEnumerable<EquipamentoDto>> ObterTodosAsync()
    {
        var equipamentos = await _repositorio.ObterTodosAsync();
        return equipamentos.Select(p => p.ToDto()); 
    }

    public async Task<EquipamentoDto> ObterPorIdAsync(Guid id)
    {
        var equipamento = await _repositorio.ObterPorIdAsync(id);
        return equipamento.ToDto();
    }

    public async Task AdicionarAsync(EquipamentoDto dto)
    {
        var equipamento = dto.ToEntity();
        await _repositorio.AdicionarAsync(equipamento);
    }

    public async Task AtualizarAsync(EquipamentoDto dto)
    {
        var equipamento = await _repositorio.ObterPorIdAsync(dto.Id);
        if (equipamento == null) return;

        equipamento.UpdateEntity(dto);
        equipamento.DataAlteracao = DateTime.UtcNow;

        await _repositorio.AtualizarAsync(equipamento);
    }

    public async Task RemoverAsync(Guid id)
    {
        var equipamento = await _repositorio.ObterPorIdAsync(id);
        if (equipamento == null) return;

        equipamento.Status = Domain.Enums.Status.Inativo;
        equipamento.DataAlteracao = DateTime.UtcNow;

        await _repositorio.AtualizarAsync(equipamento);
    }
}
