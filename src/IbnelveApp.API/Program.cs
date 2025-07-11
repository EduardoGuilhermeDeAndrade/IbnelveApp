using IbnelveApp.Application.Interfaces;
using IbnelveApp.Application.Services;
using IbnelveApp.Infrastructure.Data;
using IbnelveApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);




// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Outros serviços e repositórios
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

//app.UseHttpsRedirection();
app.UseAuthorization(); // Boa prática adicionar

app.MapControllers(); // <-- Esta linha mapeia todos os seus controllers

app.Run();

