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

// Configuração da Autenticação JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"] ?? throw new ArgumentNullException("JwtSettings:SecretKey não configurado");

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

    // Configurações adicionais para melhor segurança
    options.RequireHttpsMetadata = builder.Environment.IsProduction();
    options.SaveToken = true;

    // Eventos para logging e debugging
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError("Falha na autenticação JWT: {Error}", context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            var username = context.Principal?.Identity?.Name;
            logger.LogInformation("Token JWT validado para usuário: {Username}", username);
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

// Configuração das Políticas de Autorização
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("GerenteOuSuperior", policy => policy.RequireRole("Admin", "Gerente"));
    options.AddPolicy("UsuarioAutenticado", policy => policy.RequireAuthenticatedUser());

    // Políticas baseadas em claims
    options.AddPolicy("TenantAccess", policy =>
        policy.RequireClaim("tenant_id"));

    // Política combinada
    options.AddPolicy("AdminOuGerente", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin") ||
            context.User.IsInRole("Gerente")));
});

// Add services
builder.Services.AddControllers(options =>
{
    // Configurações globais dos controllers
    options.SuppressAsyncSuffixInActionNames = false;
});

// Configuração do Swagger com JWT
builder.Services.AddSwaggerWithJwt();

// Configuração do Entity Framework
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registro dos serviços de aplicação
builder.Services.AddScoped<IEquipamentoRepository, EquipamentoRepository>();
builder.Services.AddScoped<IEquipamentoService, EquipamentoService>();

// Registro dos serviços de autenticação
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Configuração de CORS
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

// Configuração de logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

if (builder.Environment.IsProduction())
{
    builder.Logging.AddEventLog();
}

// Configuração de health checks
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

// Middleware de segurança
app.UseHttpsRedirection();

// CORS
app.UseCors(app.Environment.IsDevelopment() ? "AllowAll" : "Production");

// Middleware customizado para tratamento de exceções globais
app.UseGlobalExceptionMiddleware();

// Configuração do Swagger com JWT
app.UseSwaggerWithJwt();

// Middleware de roteamento
app.UseRouting();

// Middlewares de segurança (a ordem é MUITO importante)
app.UseAuthentication(); // 1. Identifica o usuário a partir do token
app.UseAuthorization();  // 2. Verifica se o usuário identificado tem permissão

// Health checks
app.MapHealthChecks("/health");

// Mapeamento dos controllers
app.MapControllers();

// Endpoint de informações da API
app.MapGet("/", () => new
{
    Application = "IbnelveApp API",
    Version = "1.0.0",
    Environment = app.Environment.EnvironmentName,
    Timestamp = DateTime.UtcNow,
    Documentation = "/swagger"
});

// Middleware para logging de requisições
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Requisição: {Method} {Path}", context.Request.Method, context.Request.Path);

    await next();

    logger.LogInformation("Resposta: {StatusCode}", context.Response.StatusCode);
});

app.Run();

