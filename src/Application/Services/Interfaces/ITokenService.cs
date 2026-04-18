
namespace TiendaUCN.src.Application.Services.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateAccessTokenAsync(int user, string roleName);
        Task AddToBlacklistAsync(string token);
        Task<bool> IsTokenBlacklistedAsync(string token);
    }
}