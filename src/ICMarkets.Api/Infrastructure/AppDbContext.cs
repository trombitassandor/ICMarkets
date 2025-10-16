using ICMarkets.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace ICMarkets.Api.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<BlockchainSnapshot> BlockchainSnapshots { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BlockchainSnapshot>().HasIndex(blockchainSnapshot => 
            new
            {
                blockchainSnapshot.Chain, 
                blockchainSnapshot.CreatedAt
            });
    }
}
