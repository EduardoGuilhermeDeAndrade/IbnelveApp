using IbnelveApp.Application.Interfaces;
using IbnelveApp.Application.Services;
using IbnelveApp.Infrastructure.Data;
using IbnelveApp.Infrastructure.Repositories;
using IbnelveApp.Infrastructure.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using IbnelveApp.Application.Interfaces.Repositorios;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

// Configura��o da Autentica��o JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters { /* ... */ };
});

// Configura��o das Pol�ticas de Autoriza��o (Claims/Roles)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("GerenteOuSuperior", policy => policy.RequireRole("Admin", "Gerente"));
});


// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Outros servi�os e reposit�rios
builder.Services.AddScoped<IEquipamentoRepositorio, EquipamentoRepositorio>();
builder.Services.AddScoped<IEquipamentoService, EquipamentoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // 2. Adiciona o middleware para servir a interface do Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionMiddleware();
// Middlewares de seguran�a (a ordem � MUITO importante)
app.UseAuthentication(); // 1. Identifica o usu�rio a partir do token.
app.UseAuthorization();  // 2. Verifica se o usu�rio identificado tem permiss�o.

app.MapControllers(); // <-- Esta linha mapeia todos os seus controllers

app.Run();

