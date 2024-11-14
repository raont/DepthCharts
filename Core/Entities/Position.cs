using System.Diagnostics.CodeAnalysis;

namespace Core.Entities;

[ExcludeFromCodeCoverage]
public partial class Position
{
    public int Position1 { get; set; }

    public string? Depth { get; set; }

    public int Game { get; set; }

    public int PlayerId { get; set; }

    public virtual Game GameNavigation { get; set; } = null!;
}
