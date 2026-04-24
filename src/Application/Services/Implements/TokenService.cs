using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TiendaUCN.src.Application.Services.Interfaces;
using TiendaUCN.src.Domain.Models;
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

        public Task<string> GenerateAccessTokenAsync(User userId, string roleName)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.Id.ToString()),
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

            Log.Information("Token generado para el usuario {UserId}", userId.Id);    
            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public async Task AddToBlacklistAsync(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value 
                ?? throw new InvalidOperationException("El token no contiene un claim 'jti' valido para ser agregado a la blacklist.");
            var expiresAt = jwtToken.ValidTo;

            var IsBlacklistedToken = await _tokenRepository.IsTokenBlacklistedAsync(jti);
            if (IsBlacklistedToken)
            {
                Log.Warning("Intento de agregar un token ya en la blacklist: {Jti}", jti);
                throw new InvalidOperationException("El token ya está en la blacklist.");
            }
            var BlacklistedToken = new JwtBlacklist
            {
                TokenId = jti,
                ExpiresAt = expiresAt
            };

            await _tokenRepository.AddToBlacklistAsync(BlacklistedToken);
        }

        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
           var TokenHnadler = new JwtSecurityTokenHandler();
           var jwtToken = TokenHnadler.ReadJwtToken(token);

           var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value ;
           if (jti != null)
           {
                var isBlacklisted = await _tokenRepository.IsTokenBlacklistedAsync(jti);
                return isBlacklisted;
            
           }
           Log.Warning("El token proporcionado no contiene un claim 'jti' válido para verificar en la blacklist.");
           throw new InvalidOperationException("El token no contiene un 'jti' válido");
        }
    }
}