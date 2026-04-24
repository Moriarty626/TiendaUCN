
using TiendaUCN.src.Domain.Models;

namespace TiendaUCN.src.Application.Services.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateAccessTokenAsync(User userId, string roleName);
        Task AddToBlacklistAsync(string token);
        Task<bool> IsTokenBlacklistedAsync(string token);
    }
}