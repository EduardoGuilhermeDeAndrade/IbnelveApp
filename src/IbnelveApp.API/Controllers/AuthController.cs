using IbnelveApp.Application.DTOs.Auth;
using IbnelveApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IbnelveApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) { _authService = authService; }

        //[HttpPost("login")]
        //[ProducesResponseType(typeof(ResponseDto<string>), 200)]
        //[ProducesResponseType(typeof(ResponseDto<string>), 400)]
        //public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        //{
        //    //var response = await _authService.GerarTokenJwt(loginDto);
        //    //if (!response.Success)
        //    //{
        //    //    return BadRequest(response); // Retorna 400 com a mensagem de erro
        //    //}
        //    //return Ok(response); // Retorna 200 com o token
        //}
    }

}
