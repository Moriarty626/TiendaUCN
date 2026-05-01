namespace TiendaUCN.src.Domain.JwtBlacklist
{
    public class JwtBlacklist
    {
        public int Id { get; set; }
        public required string TokenId { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime InvalidatedAt { get; set; } = DateTime.UtcNow;
    }
}