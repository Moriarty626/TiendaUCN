using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TiendaUCN.Domain.Models.User;
using TiendaUCN.src.Application.Services.Interfaces;
using TiendaUCN.src.Domain.JwtBlacklist;
using TiendaUCN.src.Domain.Models;
using TiendaUCN.src.Infrastructure.Data.Repository.Implements;

namespace TiendaUCN.Application.Services.Implements
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

        public string GenerateAccessTokenAsync(User userId, string roleName)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userId.Id.ToString()),
                    new Claim(ClaimTypes.Role, roleName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat,
                        DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                        ClaimValueTypes.Integer64)
                };
                var secretBytes = Encoding.UTF8.GetBytes(_jwtSecret);
                Log.Information("JWT Secret size: {SecretSize} bits", secretBytes.Length * 8);

                var key = new SymmetricSecurityKey(secretBytes);
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes),
                    signingCredentials: credentials
                );

                Log.Information("Token JWT generado exitosamente para el usuario {UserId}", userId.Id);
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)

            {
                Log.Error(ex, "Error al generar el token para el usuario {UserId}", userId.Id);
                throw new InvalidOperationException("No se pudo generar el token de acceso.");
            }


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

            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            if (jti != null)
            {
                var isBlacklisted = await _tokenRepository.IsTokenBlacklistedAsync(jti);
                return isBlacklisted;

            }
            Log.Warning("El token proporcionado no contiene un claim 'jti' válido para verificar en la blacklist.");
            throw new InvalidOperationException("El token no contiene un 'jti' válido");
        }

        public async Task<int> DeleteExpiredTokensInBlacklistAsync()
        {
            int deletedCount = await _tokenRepository.PurgeExpiredTokensAsync();
            Log.Information("Tokens expirados eliminados de la blacklist: {DeletedCount}", deletedCount);
            return deletedCount;

        }
    }
}