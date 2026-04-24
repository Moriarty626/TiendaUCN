using TiendaUCN.src.Domain.Models;


namespace TiendaUCN.src.Infrastructure.Data.Repository.Implements
{
    public interface ITokenRepository
    {
        Task AddToBlacklistAsync(JwtBlacklist token);
        Task<bool> IsTokenBlacklistedAsync(string tokenId);
        Task<int> PurgeExpiredTokensAsync();
    }
}