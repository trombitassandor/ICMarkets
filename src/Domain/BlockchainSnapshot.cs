using System.ComponentModel.DataAnnotations;

namespace ICMarkets.Domain;

public class BlockchainSnapshot
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Chain { get; set; } = null!;
    public string ApiResponseJson { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
