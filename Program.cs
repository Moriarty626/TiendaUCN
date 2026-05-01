using DotNetEnv;
using Hangfire;
using Hangfire.Storage.SQLite;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Resend;
using Serilog;
using System.Text;
using TiendaUCN.Application.Services.Implements;
using TiendaUCN.Application.Services.Interfaces;
using TiendaUCN.Infrastructure.Data.Migrations;
using TiendaUCN.Infrastructure.Data.Repository.Implements;
using TiendaUCN.src.API.Middlewares;
using TiendaUCN.src.Application.DTOs.BrandDTO;
using TiendaUCN.src.Application.Mappers;
using TiendaUCN.src.Application.Services.Implements;
using TiendaUCN.src.Application.Services.Interfaces;
using TiendaUCN.src.Domain.Models;
using TiendaUCN.src.Infrastructure.Data;
using TiendaUCN.src.Infrastructure.Data.Repository.Implements;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

#region Logging Configuration
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services));
#endregion

#region Database Configuration
Log.Information("Configurando base de datos SQLite");
string connectionStringDB = Environment.GetEnvironmentVariable("DATA_BASE_URL")
    ?? throw new ArgumentNullException("DATA_BASE_URL no puede ser nulo.");
builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(connectionStringDB));
#endregion

#region JWT Authentication Configuration
Log.Information("Configurando autenticación JWT");
string jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? throw new ArgumentNullException("JWT_SECRET no puede ser nulo.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();
#endregion

#region Services and Repositories
builder.Services.AddScoped<UserMapper>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
#endregion

#region Email Service Configuration
Log.Information("Configurando servicio de correo electrónico Resend");
builder.Services.AddOptions();
builder.Services.AddHttpClient<ResendClient>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.Configure<ResendClientOptions>(o =>
{
    o.ApiToken = Environment.GetEnvironmentVariable("RESEND_API_KEY")
        ?? throw new ArgumentNullException("RESEND_API_KEY no está configurado.");
});
builder.Services.AddTransient<IResend, ResendClient>();
#endregion

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSQLiteStorage("Data Source=database.db"));


builder.Services.AddHangfireServer();

var app = builder.Build();

// 3. Activa la interfaz gráfica (Dashboard) de Hangfire
app.UseHangfireDashboard();

// 4. Programa la limpieza de usuarios no verificados (Todos los días a medianoche)
RecurringJob.AddOrUpdate<TiendaUCN.src.Infrastructure.Services.HangfireService>(
    "borrar-usuarios-no-verificados",
    servicio => servicio.CleanUnverifiedUsersJob(),
    Cron.Daily);

// 5. Programa la limpieza de la Blacklist (Todos los días a medianoche)
RecurringJob.AddOrUpdate<TiendaUCN.src.Infrastructure.Services.HangfireService>(
    "limpiar-blacklist-jwt",
    servicio => servicio.CleanExpiredTokensJob(),
    Cron.Daily);

app.MapOpenApi();

#region Database Migration
Log.Information("Aplicando migraciones a la base de datos");
using (var scope = app.Services.CreateScope())
{
    await DataSeeder.Initialize(scope.ServiceProvider);
    MapperExtensions.ConfigureMapster(scope.ServiceProvider);
}
#endregion

app.UseMiddleware<ExceptionHandilingMiddleware>(); // Primero: manejo de excepciones
app.UseMiddleware<BlacklistMiddleware>();           // Segundo: validar blacklist
app.UseAuthentication();                           // Tercero: autenticación JWT
app.UseAuthorization();                            // Cuarto: autorización por rol
app.MapControllers();
app.Run();