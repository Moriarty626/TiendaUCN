
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Resend;
using Serilog;
using TiendaUCN.src.API.Middlewares;
using TiendaUCN.src.Application.Mappers;
using TiendaUCN.src.Application.Services.Implements;
using TiendaUCN.src.Application.Services.Interfaces; //a
using TiendaUCN.src.Domain.Models;
using TiendaUCN.src.Infrastructure.Data;
using TiendaUCN.src.Infrastructure.Data.Repository.Implements;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

//Configuración de mapeadores
builder.Services.AddScoped<UserMapper>();


// Configuración de servicios y repositorios

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();


#region Logging Configuration
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services));
#endregion

#region Database Configuration
Log.Information("Configurando base de datos SQLite");
string connectionStringDB = Environment.GetEnvironmentVariable("DATA_BASE_URL") ?? throw new ArgumentNullException("DataBase name cannot be null");
builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(connectionStringDB));
#endregion


#region Email Service Configuration

Log.Information("Configurando servicio de correo electrónico Resend");
builder.Services.AddOptions();

builder.Services.AddHttpClient<ResendClient>();
builder.Services.Configure<ResendClientOptions>(o =>
{
    o.ApiToken = Environment.GetEnvironmentVariable("RESEND_API_KEY")
        ?? throw new ArgumentNullException("RESEND_API_KEY is not set");
});
builder.Services.AddTransient<IResend, ResendClient>();

#endregion

var app = builder.Build();
app.MapOpenApi();
#region Database Migration
Log.Information("Aplicando migraciones a la base de datos");
using (var scope = app.Services.CreateScope())
{
    await DataSeeder.Initialize(scope.ServiceProvider);
    MapperExtensions.ConfigureMapster(scope.ServiceProvider);
}
#endregion


app.UseMiddleware<ExceptionHandilingMiddleware>(); //Debe ir primero que todo
app.MapControllers();
app.Run();