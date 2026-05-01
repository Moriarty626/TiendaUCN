using Microsoft.EntityFrameworkCore;
using TiendaUCN.Infrastructure.Data.Migrations;
using TiendaUCN.src.Infrastructure.Data;


namespace TiendaUCN.src.Infrastructure.Services;

public class HangfireService(DataContext context)
{
    public async Task CleanExpiredTokensJob()
    {
        var now = DateTime.UtcNow;
        // Asegúrate de que en tu DataContext el DbSet se llame JwtBlacklist
        var expiredTokens = await context.JwtBlacklist
            .Where(t => t.ExpiresAt <= now)
            .ToListAsync();

        if (expiredTokens.Any())
        {
            context.JwtBlacklist.RemoveRange(expiredTokens);
            await context.SaveChangesAsync();
        }
    }

    public async Task CleanUnverifiedUsersJob()
    {
        // Definimos el límite de 24 horas para considerar a un usuario como "fantasma"
        var limitDate = DateTime.UtcNow.AddDays(-1);

        var ghostUsers = await context.Users
            .Where(u => !u.EmailConfirmed && u.CreatedAt <= limitDate) // Cambiado: EmailConfirmed en lugar de IsVerified
            .ToListAsync();

        if (ghostUsers.Any())
        {
            context.Users.RemoveRange(ghostUsers);
            await context.SaveChangesAsync();
        }
    }
}