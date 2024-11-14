using Application.Exceptions;
using Application.Validations;
using AutoMapper;
using Core.Entities;
using Infrastructure.Service;
using MediatR;

namespace Application.Command;

public class PlayerPositionRequest : IRequest
{
    public int? Position { get; set; }

    public string? DepthChart { get; set; }

    public int Game { get; set; }

    public int PlayerId { get; set; }
}

public class PlayerDepthHandler : IRequestHandler<PlayerPositionRequest>
{
    private readonly IDbHandler _dbHandler;
    private readonly IMapper _mapper;

    public PlayerDepthHandler(IDbHandler dbHandler, IMapper mapper)
    {
        _dbHandler = dbHandler;
        _mapper = mapper;
    }
    public async Task Handle(PlayerPositionRequest request, CancellationToken cancellationToken)
    {
        await GameValidations.CheckIfValidGame(_dbHandler, request.Game);
        await GameValidations.CheckPlayerRegisteredToGame(_dbHandler, request.Game, request.PlayerId);
        await DepthValidations.PlayerExistsInPosition(_dbHandler, request.Game, request.PlayerId, request.DepthChart);

        var lowestPosition = await _dbHandler.GetLowestDepth(request.Game, request.DepthChart);

        bool shiftPlayers = false;
        if (request.Position is null)
        {
            request.Position = lowestPosition is null ? 0 : lowestPosition + 1;
        }
        else if (lowestPosition != null && lowestPosition > 0 && request.Position <= lowestPosition && await _dbHandler.PlayerExistsAtPosition(request.Position.Value, request.Game, request.DepthChart))
        {
            shiftPlayers = true;
        }

        await InsertPlayer(request, shiftPlayers);
    }

    private async Task InsertPlayer(PlayerPositionRequest request, bool shiftPosition)
    {
        var newPlayerDepth = _mapper.Map<Position>(request);
        if (shiftPosition)
            await _dbHandler.ReplacePlayerPosition(newPlayerDepth);
        else
            await _dbHandler.AddPlayerPosition(newPlayerDepth);
    }
}
    
