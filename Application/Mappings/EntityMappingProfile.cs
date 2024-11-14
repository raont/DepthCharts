using Application.Command;
using Application.Models;
using AutoMapper;
using Core.Entities;
using System.Diagnostics.CodeAnalysis;

namespace Application.Mappings;

[ExcludeFromCodeCoverage]
public class EntityMappingProfile : Profile
{
    public EntityMappingProfile()
    {
        CreateMap<AddPlayerRequest, Player>()
            .ForMember(dest => dest.PlayerId, opt => opt.MapFrom(src => src.PlayerId))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.GameId, opt => opt.MapFrom(src => src.GameId));

        CreateMap<PlayerPositionRequest, Position>()
            .ForMember(dest => dest.PlayerId, opt => opt.MapFrom(src => src.PlayerId))
            .ForMember(dest => dest.Depth, opt => opt.MapFrom(src => src.DepthChart))
            .ForMember(dest => dest.Position1, opt => opt.MapFrom(src => src.Position))
            .ForMember(dest => dest.Game, opt => opt.MapFrom(src => src.Game));

        CreateMap<Player, PlayerDto>()
            .ForMember(dest => dest.PlayerId, opt => opt.MapFrom(src => src.PlayerId))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name.Split(',',StringSplitOptions.None)[0]))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Name.Split(',', StringSplitOptions.None)[1]));
    }
}
