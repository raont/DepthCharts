using Application.Exceptions;
using AutoMapper;
using Infrastructure.Service;
using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace Application.Validations;

[ExcludeFromCodeCoverage]
public static class GameValidations
{
    
    public static async Task CheckIfValidGame(IDbHandler dbHandler, int gameId)
    {
        if (!await dbHandler.GameExists(gameId))
            throw new NotFoundException("Game not found, Please setup the game first then try again");
    }

    public static async Task CheckPlayerRegisteredToGame(IDbHandler dbHandler, int gameId, int playerId)
    {
        if (!await dbHandler.PlayerExistsInGame(gameId, playerId))
            throw new NotFoundException("Player not found, Please add player to the game and then try again");
    }
}
