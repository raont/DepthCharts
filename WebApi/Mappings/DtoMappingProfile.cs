using Application.Command;
using Application.Models;
using AutoMapper;
using System.Diagnostics.CodeAnalysis;

namespace WebApi.Mappings;

[ExcludeFromCodeCoverage]
public class DtoMappingProfile : Profile
{
    public DtoMappingProfile()
    {
        CreateMap<PlayerDto, AddPlayerRequest>()
            .ForMember(dest => dest.PlayerId, opt => opt.MapFrom(src => src.PlayerId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.LastName + ", "+ src.FirstName));

        CreateMap<PlayerPositionDto, PlayerPositionRequest>()
            .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
            .ForMember(dest => dest.PlayerId, opt => opt.MapFrom(src => src.PlayerId))
            .ForMember(dest => dest.DepthChart, opt => opt.MapFrom(src => src.DepthChart.ToUpper()));

        CreateMap<SelectedPositionDto, RemovePlayerRequest>()
            .ForMember(dest => dest.DepthChart, opt => opt.MapFrom(src => src.DepthChart.ToUpper()))
            .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position));

        CreateMap<SelectedPositionDto, GetBackupRequest>()
            .ForMember(dest => dest.DepthChart, opt => opt.MapFrom(src => src.DepthChart.ToUpper()))
            .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position));
    }
}
