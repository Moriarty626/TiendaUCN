using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaUCN.src.Domain.Models
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