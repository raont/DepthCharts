using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Infrastructure.Service;

public interface IDbHandler
{
    Task AddGame(Game newGame);
    Task AddPlayer(Player newPlayer);
    Task AddPlayerPosition(Position playerPosition);
    Task ReplacePlayerPosition(Position playerPosition);

    Task<int?> DeletePlayerPosition(int positionId, int gameId, string depth);


    Task<bool> GameExists(int gameId);
    
    /// <summary>
    /// Gets the lowest position of the Position Depth lane. 
    /// Note : Lowest position is the highest number
    /// </summary>
    /// <param name="gameId">Game id</param>
    /// <param name="Depth">Name of the depth lane</param>
    /// <returns></returns>
    Task<int?> GetLowestDepth(int gameId, string Depth);

    Task<Player?> GetPlayer(int gameId, int playerId);

    Task<bool> PositionDepthExists(int gameId, string depth);
    Task<bool> PlayerExistsAtPosition(int position, int gameId, string depth);

    Task<bool> PlayerExistsInGame(int gameId, int playerId);

    Task<bool> PlayerExistsInPosition(int gameId, int playerId, string depth);

    Task<List<Player>> GetPlayersAfterCurrentPosition(int positionId, int gameId, string depth);

    Task<int?> GetGameId(string game);

    Task<PlayerDepthChart> GetPositionChartDetails(int gameId);
}

public class DbHandler : IDbHandler
{
    public readonly PlayerDepthsContext _playerDepthsContext;

    public DbHandler(PlayerDepthsContext playerDepthsContext)
    {
        _playerDepthsContext = playerDepthsContext;
    }

    public async Task AddGame(Game newGame)
    {
        await _playerDepthsContext.Games.AddAsync(newGame);
        await _playerDepthsContext.SaveChangesAsync();
    }

    public async Task AddPlayer(Player newPlayer)
    {
        await _playerDepthsContext.Players.AddAsync(newPlayer);
        await _playerDepthsContext.SaveChangesAsync();
    }

    public async Task AddPlayerPosition(Position playerPosition)
    {
        await _playerDepthsContext.Positions.AddAsync(playerPosition);
        await _playerDepthsContext.SaveChangesAsync();
    }

    public async Task ReplacePlayerPosition(Position playerPosition)
    {
        using var transaction = _playerDepthsContext.Database.BeginTransaction();

        try
        {
            _playerDepthsContext.Positions.Where(s => s.Game == playerPosition.Game && s.Depth.Equals(playerPosition.Depth) && s.Position1 >= playerPosition.Position1)
                        .ExecuteUpdate(u => u.SetProperty(e => e.Position1, e => e.Position1 + 1));
            await _playerDepthsContext.SaveChangesAsync();

            await AddPlayerPosition(playerPosition);

            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw;
        }

    }

    public async Task<int?> DeletePlayerPosition(int positionId, int gameId, string depth)
    {
        var position = _playerDepthsContext.Positions.Where(s => s.Game == gameId && s.Depth.Equals(depth) && s.Position1 == positionId);
        if (position.Any())
        {
            var payerId = position.FirstOrDefault();
            await position.ExecuteDeleteAsync();
            return payerId?.PlayerId;
        }

        return null;

    }


    public async Task<bool> GameExists(int gameId)
    {
        return await _playerDepthsContext.Games.AnyAsync(s => s.GameId == gameId);
    }

    public async Task<bool> PlayerExistsInGame(int gameId, int playerId)
    {
        return await _playerDepthsContext.Players.AnyAsync(s => s.GameId == gameId && s.PlayerId == playerId);
    }

    public async Task<bool> PlayerExistsInPosition(int gameId, int playerId, string depth)
    {
        return await _playerDepthsContext.Positions.AnyAsync(s => s.Game == gameId && s.PlayerId == playerId && s.Depth == depth);
    }

    public async Task<int?> GetLowestDepth(int gameId, string depth)
    {
        var positions = _playerDepthsContext.Positions.Where(s => s.Game == gameId && s.Depth.Equals(depth));
        if (positions.Any())
        {
            return await positions.MaxAsync(s => s.Position1);
        }

        return null;

    }

    public async Task<Player?> GetPlayer(int gameId, int playerId)
    {
        return await _playerDepthsContext.Players.FirstOrDefaultAsync(s => s.GameId == gameId && s.PlayerId == playerId);
    }

    public async Task<bool> PositionDepthExists(int gameId, string depth)
    {
        return await _playerDepthsContext.Positions.AnyAsync(s => s.Game == gameId && s.Depth.Equals(depth));
    }

    public async Task<bool> PlayerExistsAtPosition(int position, int gameId, string depth)
    {
        return await _playerDepthsContext.Positions.AnyAsync(s => s.Game == gameId && s.Depth.Equals(depth) && s.Position1 == position);
    }

    public async Task<List<Player>> GetPlayersAfterCurrentPosition(int positionId, int gameId, string depth)
    {
        var positions = _playerDepthsContext.Positions.Where(s => s.Game == gameId && s.Depth.Equals(depth)).OrderBy(s => s.Position1).ToList();
        
        var result = new List<Player>();
        
        var currentPosition = positions.Where(s => s.Position1 == positionId).SingleOrDefault();
        if (currentPosition == null)
            return result;

        var currentIndex = positions.IndexOf(currentPosition);

        foreach (var position in positions.Skip(currentIndex + 1))
        {
            var player = await GetPlayer(gameId, position.PlayerId);
            result.Add(player);
        }

        return result;
    }

    public async Task<PlayerDepthChart> GetPositionChartDetails(int gameId)
    {
        var playerDepthChart = new PlayerDepthChart();
        playerDepthChart.Positions = _playerDepthsContext.Positions.Where(s => s.Game == gameId).ToList();

        foreach (var position in playerDepthChart.Positions)
        {
            var player = await GetPlayer(gameId, position.PlayerId);
            playerDepthChart.Players.Add(player);
        }

        return playerDepthChart;
    }

    public async Task<int?> GetGameId(string game)
    {
        var selecteGame = await _playerDepthsContext.Games.FirstAsync(s => s.Name == game);
        return selecteGame?.GameId;
    }
}
