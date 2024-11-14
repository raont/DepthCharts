using System.Diagnostics.CodeAnalysis;

namespace Core.Entities;

[ExcludeFromCodeCoverage]
public partial class Game
{
    public int GameId { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();

    public virtual ICollection<Position> Positions { get; set; } = new List<Position>();
}
