using System;
using System.Diagnostics.CodeAnalysis;
namespace Core.Entities;

[ExcludeFromCodeCoverage]
public class PlayerDepthChart
{
    public ICollection<Position> Positions { get; set; } = new List<Position>();
    public ICollection<Player> Players { get; set; } = new List<Player>();
}
