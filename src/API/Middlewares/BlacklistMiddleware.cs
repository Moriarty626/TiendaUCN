     using Microsoft.EntityFrameworkCore;
using TiendaUCN.src.Application.Services.Implements;
using TiendaUCN.src.Application.Services.Interfaces;
using TiendaUCN.src.Infrastructure.Data;

namespace TiendaUCN.src.API.Middlewares;

public class BlacklistMiddleware
{
    private readonly RequestDelegate _next;

    public BlacklistMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, DataContext dbContext)
    {
        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (!string.IsNullOrEmpty(token))
        {
            var isBlacklisted = await dbContext.JwtBlacklist.AnyAsync(t => t.TokenId == token);
            if (isBlacklisted)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token revocado. Por favor, inicie sesión nuevamente.");
                return;
            }
        }
        await _next(context);
    }
}