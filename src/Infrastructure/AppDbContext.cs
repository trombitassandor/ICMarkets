using ICMarkets.Domain;
using Microsoft.EntityFrameworkCore;

namespace ICMarkets.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
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
