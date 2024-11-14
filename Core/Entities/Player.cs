using System.Diagnostics.CodeAnalysis;

namespace Core.Entities;

[ExcludeFromCodeCoverage]
public partial class Player
{
    public int PlayerId { get; set; }

    public string? Name { get; set; }

    public int GameId { get; set; }

    public virtual Game Game { get; set; } = null!;

}
