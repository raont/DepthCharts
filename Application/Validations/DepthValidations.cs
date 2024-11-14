using Application.Exceptions;
using Infrastructure.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validations;

public static class DepthValidations
{
    [ExcludeFromCodeCoverage]
    public static async Task PlayerExistsInPosition(IDbHandler dbHandler, int gameId, int playerId, string depth)
    {
        if (await dbHandler.PlayerExistsInPosition(gameId, playerId, depth))
            throw new ValidationException("Player already present in the Depth Lane, Please remove first then insert again");
    }
}
