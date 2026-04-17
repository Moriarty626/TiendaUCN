namespace TiendaUCN.Models
{
    public class JwtBlacklist
    {
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public DateTime InvalidatedAt { get; set; } = DateTime.UtcNow;
    }
}