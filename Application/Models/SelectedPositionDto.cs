using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Application.Models;

[ExcludeFromCodeCoverage]
public class SelectedPositionDto
{
    public int Position { get; set; }

    [Required]
    [MaxLength(20)]
    [MinLength(1)]
    public string DepthChart { get; set; }
}
