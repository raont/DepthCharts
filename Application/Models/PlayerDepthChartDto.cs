using System.Diagnostics.CodeAnalysis;

namespace Application.Models;

[ExcludeFromCodeCoverage]
public class PlayerDepthChartDto
{
    public List<PositionDto> Positions { get; set; }
}
