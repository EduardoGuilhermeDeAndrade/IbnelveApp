using IbnelveApp.Application.DTOs;
using IbnelveApp.Domain.Entities;

namespace IbnelveApp.Application.Mappings;

public static class EquipamentoMapper
{
    // Método para converter uma Entidade Equipamento para um EquipamentoDto
    public static EquipamentoDto ToDto(this Equipamento equipamento)
    {
        if (equipamento is null)
        {
            return null;
        }

        return new EquipamentoDto
        {
            Id = equipamento.Id,
            Nome = equipamento.Nome,
            Observacoes = equipamento.Observacoes,
            NumeroControle = equipamento.NumeroControle
        };
    }

    // Método para converter um EquipamentoDto para uma Entidade Equipamento
    public static Equipamento ToEntity(this EquipamentoDto equipamentoDto)
    {
        if (equipamentoDto is null)
        {
            return null;
        }

        return new Equipamento
        {
            Id = equipamentoDto.Id,
            Nome = equipamentoDto.Nome,
            Observacoes = equipamentoDto.Observacoes,
            NumeroControle = equipamentoDto.NumeroControle
        };
    }

    // Método para atualizar uma entidade existente a partir de um DTO
    public static void UpdateEntity(this Equipamento equipamento, EquipamentoDto equipamentoDto)
    {
        if (equipamento is null || equipamentoDto is null)
        {
            return;
        }

        equipamento.Nome = equipamentoDto.Nome;
        equipamento.Observacoes = equipamentoDto.Observacoes;
        equipamento.NumeroControle = equipamentoDto.NumeroControle;
        // Não atualize o ID!
    }
}

