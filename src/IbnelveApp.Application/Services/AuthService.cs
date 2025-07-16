using IbnelveApp.Application.DTOs.Auth;
using IbnelveApp.Application.Interfaces;
using IbnelveApp.Application.Interfaces.Repositorios;
using IbnelveApp.Application.Responses;
using System.Security.Claims;

namespace IbnelveApp.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepositorio _usuarioRepository;
        //private readonly IConfiguration _configuration;

        public AuthService(IUsuarioRepositorio usuarioRepository/* ,IConfiguration configuration*/)
        {
            _usuarioRepository = usuarioRepository;
            //_configuration = configuration;
        }

        public async Task<ApiResponse<string>> GerarTokenJwt(LoginDto loginDto)
        {
            //var usuario = await _usuarioRepository.ObterPorEmailComRolesAsync(loginDto.Email);

            //if (usuario == null || !BCrypt.Net.BCrypt.Verify(loginDto.Senha, usuario.SenhaHash))
            //{
            //    return new ApiResponse<string> { Sucesso = false, Mensagem = "Email ou senha inválidos." };
            //}

            // Lógica para criar claims (UserId, TenantId, Roles)
            var claims = new List<Claim> { /* ... */ };
            // ...

            // Lógica para gerar o token JWT
            //var tokenHandler = new JwtSecurityTokenHandler();
            //var token = tokenHandler.WriteToken(/* ... */);

            return null;// new ApiResponse<string> { Sucesso = true, Dados = token, Mensagem = "Login realizado com sucesso." };
        }
    }

}
