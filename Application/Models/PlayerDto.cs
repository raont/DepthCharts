using System.Diagnostics.CodeAnalysis;

namespace Application.Models;

[ExcludeFromCodeCoverage]
public class PlayerDto
{
    public int PlayerId { get; set; }

    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }
}
