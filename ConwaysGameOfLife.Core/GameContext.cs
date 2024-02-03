using ConwaysGameOfLife.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConwaysGameOfLife.Core;

public sealed class GameContext : DbContext
{
    public DbSet<Game?> Games { get; set; }
    public DbSet<Cell> CellStates { get; set; }
    public GameContext(DbContextOptions<GameContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>()
            .HasKey(g => g.Id);
        modelBuilder.Entity<Cell>()
            .HasKey(cs => cs.Id);

        modelBuilder.Entity<Cell>()
            .HasOne(cs => cs.Game)
            .WithMany(g => g.Cells)
            .HasForeignKey(cs => cs.GameId);
   
        base.OnModelCreating(modelBuilder);
    }
}