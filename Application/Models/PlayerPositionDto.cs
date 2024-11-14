using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Application.Models;

[ExcludeFromCodeCoverage]
public class PlayerPositionDto
{
    public int? Position { get; set; }

    [Required]
    [MaxLength(20)]
    [MinLength(1)]
    public string DepthChart { get; set; }

    [Required]
    public int PlayerId { get; set; }
}
