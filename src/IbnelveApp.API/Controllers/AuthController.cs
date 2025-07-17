using IbnelveApp.Application.DTOs.Auth;
using IbnelveApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace IbnelveApp.API.Controllers
{
    /// <summary>
    /// Controller responsável pela autenticação e autorização
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            IJwtService jwtService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _jwtService = jwtService;
            _logger = logger;
        }

        /// <summary>
        /// Realiza o login do usuário
        /// </summary>
        /// <param name="loginRequest">Dados de login</param>
        /// <returns>Token JWT e dados do usuário</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Realiza login do usuário",
            Description = "Autentica o usuário e retorna um token JWT válido"
        )]
        [SwaggerResponse(200, "Login realizado com sucesso", typeof(LoginResponseDto))]
        [SwaggerResponse(400, "Dados de login inválidos", typeof(AuthErrorResponseDto))]
        [SwaggerResponse(401, "Credenciais inválidas", typeof(AuthErrorResponseDto))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(AuthErrorResponseDto))]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                _logger.LogInformation("Tentativa de login para usuário: {Username}", loginRequest.Username);

                if (!ModelState.IsValid)
                {
                    return BadRequest(new AuthErrorResponseDto
                    {
                        ErrorCode = "INVALID_REQUEST",
                        Message = "Dados de login inválidos",
                        Details = ModelState
                    });
                }

                var user = await _authService.AuthenticateAsync(loginRequest.Username, loginRequest.Password, loginRequest.TenantId);

                if (user == null)
                {
                    _logger.LogWarning("Falha na autenticação para usuário: {Username}", loginRequest.Username);
                    return Unauthorized(new AuthErrorResponseDto
                    {
                        ErrorCode = "INVALID_CREDENTIALS",
                        Message = "Usuário ou senha inválidos"
                    });
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning("Tentativa de login com usuário inativo: {Username}", loginRequest.Username);
                    return Unauthorized(new AuthErrorResponseDto
                    {
                        ErrorCode = "USER_INACTIVE",
                        Message = "Usuário inativo"
                    });
                }

                var token = _jwtService.GenerateToken(user);
                var expirationDate = _jwtService.GetTokenExpirationDate(token);

                _logger.LogInformation("Login realizado com sucesso para usuário: {Username}", loginRequest.Username);

                return Ok(new LoginResponseDto
                {
                    Token = token,
                    TokenType = "Bearer",
                    ExpiresAt = expirationDate ?? DateTime.UtcNow.AddHours(1),
                    User = user
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante o login para usuário: {Username}", loginRequest.Username);
                return StatusCode(500, new AuthErrorResponseDto
                {
                    ErrorCode = "INTERNAL_ERROR",
                    Message = "Erro interno do servidor"
                });
            }
        }

        /// <summary>
        /// Valida um token JWT
        /// </summary>
        /// <param name="token">Token a ser validado</param>
        /// <returns>Informações sobre a validade do token</returns>
        [HttpPost("validate-token")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Valida um token JWT",
            Description = "Verifica se um token JWT é válido e retorna informações do usuário"
        )]
        [SwaggerResponse(200, "Token validado", typeof(TokenValidationResponseDto))]
        [SwaggerResponse(400, "Token inválido", typeof(AuthErrorResponseDto))]
        public async Task<IActionResult> ValidateToken([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Token))
                {
                    return BadRequest(new AuthErrorResponseDto
                    {
                        ErrorCode = "INVALID_TOKEN",
                        Message = "Token é obrigatório"
                    });
                }

                var principal = _jwtService.ValidateToken(request.Token);

                if (principal == null)
                {
                    return Ok(new TokenValidationResponseDto
                    {
                        IsValid = false,
                        ErrorMessage = "Token inválido ou expirado"
                    });
                }

                var userId = _jwtService.GetUserIdFromToken(request.Token);
                var username = _jwtService.GetUsernameFromToken(request.Token);
                var roles = _jwtService.GetRolesFromToken(request.Token);
                var tenantId = _jwtService.GetTenantIdFromToken(request.Token);
                var expirationDate = _jwtService.GetTokenExpirationDate(request.Token);

                var user = new UserDto
                {
                    Id = int.Parse(userId ?? "0"),
                    Username = username ?? "",
                    Roles = roles,
                    TenantId = string.IsNullOrEmpty(tenantId) ? null : int.Parse(tenantId)
                };

                return Ok(new TokenValidationResponseDto
                {
                    IsValid = true,
                    User = user,
                    ExpiresAt = expirationDate
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante validação do token");
                return StatusCode(500, new AuthErrorResponseDto
                {
                    ErrorCode = "INTERNAL_ERROR",
                    Message = "Erro interno do servidor"
                });
            }
        }

        /// <summary>
        /// Obtém informações do usuário autenticado
        /// </summary>
        /// <returns>Dados do usuário atual</returns>
        [HttpGet("me")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Obtém dados do usuário autenticado",
            Description = "Retorna informações do usuário baseado no token JWT"
        )]
        [SwaggerResponse(200, "Dados do usuário", typeof(UserDto))]
        [SwaggerResponse(401, "Não autorizado", typeof(AuthErrorResponseDto))]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
                var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();

                var user = new UserDto
                {
                    Id = int.Parse(userId ?? "0"),
                    Username = username ?? "",
                    Email = email,
                    Roles = roles,
                    TenantId = string.IsNullOrEmpty(tenantIdClaim) ? null : int.Parse(tenantIdClaim),
                    IsActive = true
                };

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter dados do usuário atual");
                return StatusCode(500, new AuthErrorResponseDto
                {
                    ErrorCode = "INTERNAL_ERROR",
                    Message = "Erro interno do servidor"
                });
            }
        }

        /// <summary>
        /// Realiza logout do usuário (invalidação do token no lado cliente)
        /// </summary>
        /// <returns>Confirmação de logout</returns>
        [HttpPost("logout")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Realiza logout do usuário",
            Description = "Invalida o token JWT (implementação no lado cliente)"
        )]
        [SwaggerResponse(200, "Logout realizado com sucesso")]
        [SwaggerResponse(401, "Não autorizado")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                _logger.LogInformation("Logout realizado para usuário: {Username}", username);

                return Ok(new { message = "Logout realizado com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante logout");
                return StatusCode(500, new AuthErrorResponseDto
                {
                    ErrorCode = "INTERNAL_ERROR",
                    Message = "Erro interno do servidor"
                });
            }
        }

        /// <summary>
        /// Endpoint para testar autorização (apenas para usuários autenticados)
        /// </summary>
        /// <returns>Mensagem de teste</returns>
        [HttpGet("test-auth")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Testa autenticação",
            Description = "Endpoint protegido para testar se a autenticação está funcionando"
        )]
        [SwaggerResponse(200, "Autenticação funcionando")]
        [SwaggerResponse(401, "Não autorizado")]
        public IActionResult TestAuth()
        {
            var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList();

            return Ok(new
            {
                message = "Autenticação funcionando!",
                user = username,
                roles = roles,
                timestamp = DateTime.UtcNow
            });
        }
    }
}

