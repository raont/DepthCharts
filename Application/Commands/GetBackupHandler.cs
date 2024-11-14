using Application.Models;
using Application.Validations;
using AutoMapper;
using Infrastructure.Service;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace Application.Command;

[ExcludeFromCodeCoverage]
public class GetBackupRequest : IRequest<List<PlayerDto>>
{
    public int Position { get; set; }

    public string DepthChart { get; set; }

    public int Game { get; set; }
}
public class GetBackupHandler : IRequestHandler<GetBackupRequest, List<PlayerDto>>
{
    private readonly IDbHandler _dbHandler;
    private readonly IMapper _mapper;

    public GetBackupHandler(IDbHandler dbHandler, IMapper mapper)
    {
        _dbHandler = dbHandler;
        _mapper = mapper;
    }

    public async Task<List<PlayerDto>> Handle(GetBackupRequest request, CancellationToken cancellationToken)
    {
        await GameValidations.CheckIfValidGame(_dbHandler, request.Game);

        var players = await _dbHandler.GetPlayersAfterCurrentPosition(request.Position, request.Game, request.DepthChart);

        return _mapper.Map<List<PlayerDto>>(players);
    }
}
