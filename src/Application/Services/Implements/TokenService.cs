using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TiendaUCN.src.Application.Services.Interfaces;
using TiendaUCN.src.Infrastructure.Data.Repository.Implements;

namespace TiendaUCN.src.Application.Services.Implements
{
    public class TokenService : ITokenService
    {
        private readonly string _jwtSecret;
        private readonly int _jwtExpirationMinutes;
        private readonly ITokenRepository _tokenRepository;

        public TokenService(ITokenRepository tokenRepository, IConfiguration configuration)
        {
            _tokenRepository = tokenRepository;
            _jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? throw new InvalidOperationException("JWT_SECRET no está configurado en las variables de entorno.");
            _jwtExpirationMinutes = configuration.GetValue<int>("Jwt:ExpirationMinutes", 60);
        }

        public Task<string> GenerateAccessTokenAsync(int userId, string roleName)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(ClaimTypes.Role, roleName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes),
                signingCredentials: credentials
            );

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public async Task AddToBlacklistAsync(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var expiresAt = jwtToken.ValidTo;

            await _tokenRepository.AddToBlacklistAsync(token, expiresAt);
        }

        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            return await _tokenRepository.IsTokenBlacklistedAsync(token);
        }
    }
}