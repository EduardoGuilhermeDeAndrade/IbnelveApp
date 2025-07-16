using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IbnelveApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOnly")] // Apenas usuários com a role "Admin"
    public class AdminController : ControllerBase
    {
        //// ... injete IUsuarioService, ITenantService ...

        //[HttpPost("usuarios")]
        //public async Task<IActionResult> CriarUsuario([FromBody] UsuarioCreateDto dto)
        //{
        //    // Lógica para criar usuário, fazer hash da senha, etc.
        //    // Lembre-se de sempre retornar usando o padrão ResponseDto<T>
        //}

        //// ... outros endpoints para gerenciar tenants, roles, etc.
    }

}
