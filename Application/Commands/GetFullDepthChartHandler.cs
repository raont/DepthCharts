using Application.Exceptions;
using Application.Models;
using Application.Validations;
using AutoMapper;
using Core.Entities;
using Infrastructure.Service;
using MediatR;

namespace Application.Command;

public class GetDepthChartRequest : IRequest<PlayerDepthChartDto>
{ 
    public string gameName { get; set; }
}

public class GetFullDepthChartHandler : IRequestHandler<GetDepthChartRequest, PlayerDepthChartDto>
{
    private readonly IDbHandler _dbHandler;
    private readonly IMapper _mapper;

    public GetFullDepthChartHandler(IDbHandler dbHandler, IMapper mapper)
    {
        _dbHandler = dbHandler;
        _mapper = mapper;
    }
    public async Task<PlayerDepthChartDto> Handle(GetDepthChartRequest gameRequest, CancellationToken cancellationToken)
    {
        var gameId = await _dbHandler.GetGameId(gameRequest.gameName);

        if (gameId == null)
            throw new NotFoundException("Game not found");

        var positionResults = await _dbHandler.GetPositionChartDetails(gameId.Value);

        var postionNames = positionResults.Positions.Select(s => s.Depth).Distinct();

        var depthChart = new PlayerDepthChartDto();
        depthChart.Positions = new List<PositionDto>();

        foreach (var depth in postionNames)
        {
            var postionDepth = new PositionDto
            {
                PositionName = depth,
                PlayerList = []
            };

            foreach (var positionsfromDb in positionResults.Positions.Where(s => s.Depth ==  depth))
            {
                var player = positionResults.Players.FirstOrDefault(s => s.PlayerId == positionsfromDb.PlayerId);
                if(!postionDepth.PlayerList.ContainsKey(positionsfromDb.Position1))
                    postionDepth.PlayerList.Add(positionsfromDb.Position1, _mapper.Map<PlayerDto>(player));
            }

            depthChart.Positions.Add(postionDepth);
        }

        return depthChart;
    }
}
