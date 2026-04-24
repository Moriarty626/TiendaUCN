using Microsoft.EntityFrameworkCore;
using TiendaUCN.Models;
using TiendaUCN.src.Domain.Models;
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

        public async Task AddToBlacklistAsync(JwtBlacklist token)
        {
        
            await _context.JwtBlacklist.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsTokenBlacklistedAsync(string tokenId)
        {
            return await _context.JwtBlacklist.AnyAsync(t => t.TokenId == tokenId);
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