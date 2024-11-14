using Infrastructure.Service;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace Application.Command;

[ExcludeFromCodeCoverage]
public class CreateGameHandlerRequest : IRequest<int>
{ 
    public string Name { get; set; }
}

[ExcludeFromCodeCoverage]
public class CreateGameHandler : IRequestHandler<CreateGameHandlerRequest, int>
{
    private IDbHandler _dbHandler;
    public CreateGameHandler(IDbHandler dbHandler)
    {
        _dbHandler = dbHandler;
    }
    public async Task<int> Handle(CreateGameHandlerRequest request, CancellationToken cancellationToken)
    {
        var newGame = new Core.Entities.Game { Name = request.Name };
        await _dbHandler.AddGame(newGame);
        return newGame.GameId;
    }
}
