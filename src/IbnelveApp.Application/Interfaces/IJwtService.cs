using System.Security.Claims;
using IbnelveApp.Application.DTOs.Auth;

namespace IbnelveApp.Application.Interfaces
{
    public interface IJwtService
    {
        /// <summary>
        /// Gera um token JWT para o usuário especificado
        /// </summary>
        /// <param name="user">Dados do usuário</param>
        /// <returns>Token JWT como string</returns>
        string GenerateToken(UserDto user);

        /// <summary>
        /// Valida um token JWT e retorna o ClaimsPrincipal
        /// </summary>
        /// <param name="token">Token JWT para validar</param>
        /// <returns>ClaimsPrincipal se válido, null se inválido</returns>
        ClaimsPrincipal? ValidateToken(string token);

        /// <summary>
        /// Extrai o ID do usuário do token
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>ID do usuário ou null</returns>
        string? GetUserIdFromToken(string token);

        /// <summary>
        /// Extrai o nome de usuário do token
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>Nome de usuário ou null</returns>
        string? GetUsernameFromToken(string token);

        /// <summary>
        /// Extrai as roles do usuário do token
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>Lista de roles</returns>
        List<string> GetRolesFromToken(string token);

        /// <summary>
        /// Extrai o ID do tenant do token
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>ID do tenant ou null</returns>
        string? GetTenantIdFromToken(string token);

        /// <summary>
        /// Verifica se o token está expirado
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>True se expirado, false caso contrário</returns>
        bool IsTokenExpired(string token);

        /// <summary>
        /// Obtém a data de expiração do token
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <returns>Data de expiração ou null</returns>
        DateTime? GetTokenExpirationDate(string token);
    }
}

