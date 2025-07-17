using System.ComponentModel.DataAnnotations;

namespace IbnelveApp.Application.DTOs.Auth
{
    /// <summary>
    /// DTO para requisição de login
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// Nome de usuário ou email
        /// </summary>
        [Required(ErrorMessage = "Username é obrigatório")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Senha do usuário
        /// </summary>
        [Required(ErrorMessage = "Password é obrigatório")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// ID do tenant (opcional para multi-tenancy)
        /// </summary>
        public int? TenantId { get; set; }
    }

    /// <summary>
    /// DTO para resposta de login
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>
        /// Token JWT gerado
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Tipo do token (sempre "Bearer")
        /// </summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// Data de expiração do token
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Dados do usuário autenticado
        /// </summary>
        public UserDto User { get; set; } = new();
    }

    /// <summary>
    /// DTO para dados do usuário
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// ID único do usuário
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome de usuário
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email do usuário
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Nome completo do usuário
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// ID do tenant (para multi-tenancy)
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// Roles/perfis do usuário
        /// </summary>
        public List<string> Roles { get; set; } = new();

        /// <summary>
        /// Indica se o usuário está ativo
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Data de criação do usuário
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Data da última atualização
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO para refresh token
    /// </summary>
    public class RefreshTokenRequestDto
    {
        /// <summary>
        /// Token atual para renovação
        /// </summary>
        [Required(ErrorMessage = "Token é obrigatório")]
        public string Token { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para resposta de validação de token
    /// </summary>
    public class TokenValidationResponseDto
    {
        /// <summary>
        /// Indica se o token é válido
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Dados do usuário se o token for válido
        /// </summary>
        public UserDto? User { get; set; }

        /// <summary>
        /// Mensagem de erro se o token for inválido
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Data de expiração do token
        /// </summary>
        public DateTime? ExpiresAt { get; set; }
    }

    /// <summary>
    /// DTO para resposta de erro de autenticação
    /// </summary>
    public class AuthErrorResponseDto
    {
        /// <summary>
        /// Código do erro
        /// </summary>
        public string ErrorCode { get; set; } = string.Empty;

        /// <summary>
        /// Mensagem de erro
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Detalhes adicionais do erro
        /// </summary>
        public object? Details { get; set; }

        /// <summary>
        /// Timestamp do erro
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}

