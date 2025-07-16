using IbnelveApp.Application.Interfaces.Repositorios;
using IbnelveApp.Domain.Entities;
using IbnelveApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IbnelveApp.Infrastructure.Repositories;

public class UsuarioRepositorio : RepositorioBase<Usuario>, IUsuarioRepositorio
{
    public UsuarioRepositorio(AppDbContext context) : base(context) { }

    
}
