using Application.Validations;
using AutoMapper;
using Core.Entities;
using Infrastructure.Service;
using MediatR;

namespace Application.Command;

public class AddPlayerRequest : IRequest
{
    public int PlayerId { get; set; }
    public string? Name { get; set; }
    public int GameId { get; set; }
}

public class AddPlayerHandler : IRequestHandler<AddPlayerRequest>
{
    private readonly IDbHandler _dbHandler;
    private readonly IMapper _mapper;

    public AddPlayerHandler(IDbHandler dbHandler, IMapper mapper)
    {
        _dbHandler = dbHandler;
        _mapper = mapper;
    }
    public async Task Handle(AddPlayerRequest request, CancellationToken cancellationToken)
    {
        await GameValidations.CheckIfValidGame(_dbHandler, request.GameId);

        var newPlayer = _mapper.Map<Player>(request);
        await _dbHandler.AddPlayer(newPlayer);
    }
}
