namespace TiendaUCN.src.Infrastructure.Data.Repository.Implements
{
    public interface ITokenRepository
    {
        Task AddToBlacklistAsync(string token, DateTime expiresAt);
        Task<bool> IsTokenBlacklistedAsync(string token);
        Task<int> PurgeExpiredTokensAsync();
    }
}