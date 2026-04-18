using Resend;
using System.Security.Claims;
using TiendaUCN.src.Application.Services.Interfaces;
using TiendaUCN.src.Infrastructure.Data.Repository.Implements;

namespace TiendaUCN.src.Application.Services.Implements
{
    public class TokenService : ITokenService
    {
        private readonly string _jwtSecret;
        private readonly ITokenRepository _tokenRepository;
        public TokenService(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
            _jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? throw new InvalidOperationException("JWT secret key is not configured in environment variables.");

        }

        public Task AddToBlacklistAsync(string token)
        {
            throw new NotImplementedException();
        }

        public Task<string> GenerateAccessTokenAsync(int user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsTokenBlacklistedAsync(string token)
        {
            throw new NotImplementedException();
        }
    }
}