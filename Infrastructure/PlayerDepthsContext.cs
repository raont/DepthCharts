using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure;

[ExcludeFromCodeCoverage]
public partial class PlayerDepthsContext : DbContext
{
    protected readonly IConfiguration Configuration;

    public PlayerDepthsContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // connect to sqlite database
        options.UseSqlite(Configuration.GetConnectionString("WebApiDatabase"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>(entity =>
        {
            entity.ToTable("Game");

            entity.Property(e => e.GameId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => new { e.PlayerId, e.GameId });

            entity.ToTable("Player");

            entity.HasOne(d => d.Game).WithMany(p => p.Players)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => new { e.Position1, e.Game, e.PlayerId });

            entity.ToTable("Position");

            entity.Property(e => e.Position1).HasColumnName("Position");

            entity.HasOne(d => d.GameNavigation).WithMany(p => p.Positions)
                .HasForeignKey(d => d.Game)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
