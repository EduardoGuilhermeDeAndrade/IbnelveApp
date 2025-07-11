using AutoMapper;
using IbnelveApp.Application.DTOs;
using IbnelveApp.Domain.Entities;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Equipamento, EquipamentoDto>().ReverseMap();
    }
}
