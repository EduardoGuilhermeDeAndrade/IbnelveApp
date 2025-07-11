using IbnelveApp.Domain.Entities;
using IbnelveApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace IbnelveApp.Infrastructure.Data;

public class AppDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Guid? _tenantId; // Tornar o TenantId anulável

    public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
        var tenantIdClaim = httpContextAccessor.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value; // Replace FindFirstValue with FindFirst and access Value
        if (Guid.TryParse(tenantIdClaim, out var tenantId))
        {
            _tenantId = tenantId;
        }
    }

    public DbSet<Equipamento> Equipamentos { get; set; }
    public DbSet<Role> Role { get; set; }
    public DbSet<Tenant> Tenant { get; set; }
    public DbSet<Usuario> Usuario { get; set; }
    public DbSet<UsuarioRole> UsuarioRole { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // --- FILTROS GLOBAIS ---
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // 1. Filtro de Exclusão Lógica (Soft Delete)
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(ConvertFilterExpression<ISoftDelete>(e => !e.IsDeleted, entityType.ClrType));
            }

            // 2. Filtro de Multi-Tenancy
            if (typeof(IMultiTenant).IsAssignableFrom(entityType.ClrType))
            {
                // Só aplica o filtro se um tenantId foi extraído do token
                if (_tenantId.HasValue)
                {
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(ConvertFilterExpression<IMultiTenant>(e => e.TenantId == _tenantId.Value, entityType.ClrType));
                }
            }
        }
    }

    // Método auxiliar para converter a expressão de filtro
    private static LambdaExpression ConvertFilterExpression<TInterface>(
        Expression<Func<TInterface, bool>> filterExpression,
        Type entityType)
    {
        var newParam = Expression.Parameter(entityType);
        var newBody = ReplacingExpressionVisitor.Replace(filterExpression.Parameters.Single(), newParam, filterExpression.Body);
        return Expression.Lambda(newBody, newParam);
    }
}
