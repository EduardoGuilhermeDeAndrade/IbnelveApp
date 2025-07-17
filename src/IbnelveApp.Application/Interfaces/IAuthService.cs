using IbnelveApp.Application.DTOs.Auth;

namespace IbnelveApp.Application.Interfaces
{
    /// <summary>
    /// Interface para serviços de autenticação
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Autentica um usuário com username e senha
        /// </summary>
        /// <param name="username">Nome de usuário ou email</param>
        /// <param name="password">Senha do usuário</param>
        /// <param name="tenantId">ID do tenant (opcional)</param>
        /// <returns>Dados do usuário se autenticado, null caso contrário</returns>
        Task<UserDto?> AuthenticateAsync(string username, string password, int? tenantId = null);

        /// <summary>
        /// Obtém um usuário pelo ID
        /// </summary>
        /// <param name="userId">ID do usuário</param>
        /// <returns>Dados do usuário ou null se não encontrado</returns>
        Task<UserDto?> GetUserByIdAsync(int userId);

        /// <summary>
        /// Obtém um usuário pelo username
        /// </summary>
        /// <param name="username">Nome de usuário</param>
        /// <param name="tenantId">ID do tenant (opcional)</param>
        /// <returns>Dados do usuário ou null se não encontrado</returns>
        Task<UserDto?> GetUserByUsernameAsync(string username, int? tenantId = null);

        /// <summary>
        /// Verifica se um usuário tem uma role específica
        /// </summary>
        /// <param name="userId">ID do usuário</param>
        /// <param name="role">Nome da role</param>
        /// <returns>True se o usuário tem a role, false caso contrário</returns>
        Task<bool> UserHasRoleAsync(int userId, string role);

        /// <summary>
        /// Obtém todas as roles de um usuário
        /// </summary>
        /// <param name="userId">ID do usuário</param>
        /// <returns>Lista de roles do usuário</returns>
        Task<List<string>> GetUserRolesAsync(int userId);

        /// <summary>
        /// Verifica se um usuário está ativo
        /// </summary>
        /// <param name="userId">ID do usuário</param>
        /// <returns>True se o usuário está ativo, false caso contrário</returns>
        Task<bool> IsUserActiveAsync(int userId);

        /// <summary>
        /// Atualiza a data do último login do usuário
        /// </summary>
        /// <param name="userId">ID do usuário</param>
        /// <returns>Task</returns>
        Task UpdateLastLoginAsync(int userId);

        /// <summary>
        /// Valida se a senha está correta para um usuário
        /// </summary>
        /// <param name="userId">ID do usuário</param>
        /// <param name="password">Senha a ser validada</param>
        /// <returns>True se a senha está correta, false caso contrário</returns>
        Task<bool> ValidatePasswordAsync(int userId, string password);

        /// <summary>
        /// Cria um hash da senha
        /// </summary>
        /// <param name="password">Senha em texto plano</param>
        /// <returns>Hash da senha</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifica se uma senha corresponde ao hash
        /// </summary>
        /// <param name="password">Senha em texto plano</param>
        /// <param name="hash">Hash da senha</param>
        /// <returns>True se a senha corresponde ao hash, false caso contrário</returns>
        bool VerifyPassword(string password, string hash);
    }
}

