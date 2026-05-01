namespace TiendaUCN.Domain.Models.User
{
    public class VerificationCode
    {
        public int Id { get; set; }
        public required string Code { get; set; }
        public required DateTime Expiry { get; set; }
        public int FailedAttempts { get; set; } = 0;
        public DateTime DateToResend { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
    }
}