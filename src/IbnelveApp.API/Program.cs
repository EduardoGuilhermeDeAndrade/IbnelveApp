using IbnelveApp.Application.Interfaces;
using IbnelveApp.Application.Services;
using IbnelveApp.Infrastructure.Data;
using IbnelveApp.Infrastructure.Repositories;
using IbnelveApp.Infrastructure.Middleware;
using IbnelveApp.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using IbnelveApp.Application.Interfaces.Repositories;
using IbnelveApp.API.Configuration;
using System.Text;
using IbnelveApp.Application.Interfaces.Repositorios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configura��o da Autentica��o JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"] ?? throw new ArgumentNullException("JwtSettings:SecretKey n�o configurado");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "IbnelveApp",
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"] ?? "IbnelveApp",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        RequireExpirationTime = true
    };

    // Configura��es adicionais para melhor seguran�a
    options.RequireHttpsMetadata = builder.Environment.IsProduction();
    options.SaveToken = true;

    // Eventos para logging e debugging
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError("Falha na autentica��o JWT: {Error}", context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            var username = context.Principal?.Identity?.Name;
            logger.LogInformation("Token JWT validado para usu�rio: {Username}", username);
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Desafio JWT: {Error}", context.Error);
            return Task.CompletedTask;
        }
    };
});

// Configura��o das Pol�ticas de Autoriza��o
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("GerenteOuSuperior", policy => policy.RequireRole("Admin", "Gerente"));
    options.AddPolicy("UsuarioAutenticado", policy => policy.RequireAuthenticatedUser());

    // Pol�ticas baseadas em claims
    options.AddPolicy("TenantAccess", policy =>
        policy.RequireClaim("tenant_id"));

    // Pol�tica combinada
    options.AddPolicy("AdminOuGerente", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin") ||
            context.User.IsInRole("Gerente")));
});

// Add services
builder.Services.AddControllers(options =>
{
    // Configura��es globais dos controllers
    options.SuppressAsyncSuffixInActionNames = false;
});

// Configura��o do Swagger com JWT
builder.Services.AddSwaggerWithJwt();

// Configura��o do Entity Framework
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registro dos servi�os de aplica��o
builder.Services.AddScoped<IEquipamentoRepository, EquipamentoRepository>();
builder.Services.AddScoped<IEquipamentoService, EquipamentoService>();

// Registro dos servi�os de autentica��o
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Configura��o de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });

    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://yourdomain.com", "https://www.yourdomain.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Configura��o de logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

if (builder.Environment.IsProduction())
{
    builder.Logging.AddEventLog();
}

// Configura��o de health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Middleware de seguran�a
app.UseHttpsRedirection();

// CORS
app.UseCors(app.Environment.IsDevelopment() ? "AllowAll" : "Production");

// Middleware customizado para tratamento de exce��es globais
app.UseGlobalExceptionMiddleware();

// Configura��o do Swagger com JWT
app.UseSwaggerWithJwt();

// Middleware de roteamento
app.UseRouting();

// Middlewares de seguran�a (a ordem � MUITO importante)
app.UseAuthentication(); // 1. Identifica o usu�rio a partir do token
app.UseAuthorization();  // 2. Verifica se o usu�rio identificado tem permiss�o

// Health checks
app.MapHealthChecks("/health");

// Mapeamento dos controllers
app.MapControllers();

// Endpoint de informa��es da API
app.MapGet("/", () => new
{
    Application = "IbnelveApp API",
    Version = "1.0.0",
    Environment = app.Environment.EnvironmentName,
    Timestamp = DateTime.UtcNow,
    Documentation = "/swagger"
});

// Middleware para logging de requisi��es
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Requisi��o: {Method} {Path}", context.Request.Method, context.Request.Path);

    await next();

    logger.LogInformation("Resposta: {StatusCode}", context.Response.StatusCode);
});

app.Run();

