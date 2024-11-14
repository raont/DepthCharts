using System.Diagnostics.CodeAnalysis;

namespace Application.Models;

[ExcludeFromCodeCoverage]
public class PositionDto
{
    public string PositionName { get; set; }
    public SortedList<int, PlayerDto> PlayerList { get; set; }
}
