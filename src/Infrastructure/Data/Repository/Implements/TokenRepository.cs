using Microsoft.EntityFrameworkCore;
using TiendaUCN.Models;
using TiendaUCN.src.Infrastructure.Data;

namespace TiendaUCN.src.Infrastructure.Data.Repository.Implements
{
    public class TokenRepository : ITokenRepository
    {
        private readonly DataContext _context;

        public TokenRepository(DataContext context)
        {
            _context = context;
        }

        public async Task AddToBlacklistAsync(string token, DateTime expiresAt)
        {
            var blacklisted = new JwtBlacklist
            {
                Token = token,
                ExpiresAt = expiresAt,
                InvalidatedAt = DateTime.UtcNow
            };

            await _context.JwtBlacklist.AddAsync(blacklisted);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            return await _context.JwtBlacklist.AnyAsync(t => t.Token == token);
        }

        public async Task<int> PurgeExpiredTokensAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.JwtBlacklist
                .Where(t => t.ExpiresAt <= now)
                .ExecuteDeleteAsync();
        }
    }
}