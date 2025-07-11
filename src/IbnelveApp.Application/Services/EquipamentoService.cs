using AutoMapper;
using IbnelveApp.Application.DTOs;
using IbnelveApp.Application.Interfaces;
using IbnelveApp.Domain.Entities;

namespace IbnelveApp.Application.Services;

public class EquipamentoService : IEquipamentoService
{
    private readonly IEquipamentoRepositorio _repositorio;
    private readonly IMapper _mapper;

    public EquipamentoService(IEquipamentoRepositorio repositorio, IMapper mapper)
    {
        _repositorio = repositorio;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EquipamentoDto>> ObterTodosAsync()
    {
        var equipamentos = await _repositorio.ObterTodosAsync();
        return _mapper.Map<IEnumerable<EquipamentoDto>>(equipamentos);
    }

    public async Task<EquipamentoDto> ObterPorIdAsync(Guid id)
    {
        var equipamento = await _repositorio.ObterPorIdAsync(id);
        return _mapper.Map<EquipamentoDto>(equipamento);
    }

    public async Task AdicionarAsync(EquipamentoDto dto)
    {
        var equipamento = _mapper.Map<Equipamento>(dto);
        await _repositorio.AdicionarAsync(equipamento);
    }

    public async Task AtualizarAsync(EquipamentoDto dto)
    {
        var equipamento = await _repositorio.ObterPorIdAsync(dto.Id);
        if (equipamento == null) return;

        _mapper.Map(dto, equipamento);
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
