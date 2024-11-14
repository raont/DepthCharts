using Application.Models;
using Application.Validations;
using AutoMapper;
using Infrastructure.Service;
using MediatR;

namespace Application.Command;

public class RemovePlayerRequest : IRequest<PlayerDto>
{
    public int Position { get; set; }

    public string DepthChart { get; set; }

    public int Game { get; set; }
}

public class RemovePlayerHandler : IRequestHandler<RemovePlayerRequest, PlayerDto?>
{
    private readonly IDbHandler _dbHandler;
    private readonly IMapper _mapper;

    public RemovePlayerHandler(IDbHandler dbHandler, IMapper mapper)
    {
        _dbHandler = dbHandler;
        _mapper = mapper;
    }

    public async Task<PlayerDto?> Handle(RemovePlayerRequest request, CancellationToken cancellationToken)
    {
        await GameValidations.CheckIfValidGame(_dbHandler, request.Game);

        var playerId = await _dbHandler.DeletePlayerPosition(request.Position, request.Game, request.DepthChart);

        if (playerId is null)
            return null;

        var player = await _dbHandler.GetPlayer(request.Game, playerId.Value);

        if (player is null)
            return null;

        return _mapper.Map<PlayerDto>(player);
    }
}
