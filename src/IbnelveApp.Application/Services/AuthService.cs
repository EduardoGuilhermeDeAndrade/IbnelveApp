using IbnelveApp.Application.DTOs.Auth;
using IbnelveApp.Application.Interfaces;
using IbnelveApp.Application.Interfaces.Repositories;
using IbnelveApp.Domain.Entities;
using BCrypt.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace IbnelveApp.Infrastructure.Services
{
    /// <summary>
    /// Implementação do serviço de autenticação
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<AuthService> _logger;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserRepository userRepository,
            IUserRoleRepository userRoleRepository,
            IRoleRepository roleRepository,
            ILogger<AuthService> logger,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Autentica um usuário com username e senha
        /// </summary>
        public async Task<UserDto?> AuthenticateAsync(string username, string password, int? tenantId = null)
        {
            try
            {
                _logger.LogInformation("Iniciando autenticação para usuário: {Username}, TenantId: {TenantId}",
                    username, tenantId);

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    _logger.LogWarning("Tentativa de autenticação com credenciais vazias");
                    return null;
                }

                // Buscar usuário por username ou email
                var user = await GetUserEntityByUsernameAsync(username, tenantId);

                if (user == null)
                {
                    _logger.LogWarning("Usuário não encontrado: {Username}", username);
                    return null;
                }

                // Verificar se o usuário está ativo
                if (!user.IsActive)
                {
                    _logger.LogWarning("Tentativa de login com usuário inativo: {Username}", username);
                    return null;
                }

                // Verificar se a conta não está bloqueada
                if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
                {
                    _logger.LogWarning("Tentativa de login com conta bloqueada: {Username}", username);
                    return null;
                }

                // Validar senha
                if (!VerifyPassword(password, user.PasswordHash))
                {
                    _logger.LogWarning("Senha incorreta para usuário: {Username}", username);

                    // Incrementar tentativas de login falhadas
                    await IncrementFailedLoginAttemptsAsync(user);
                    return null;
                }

                // Reset das tentativas de login falhadas em caso de sucesso
                await ResetFailedLoginAttemptsAsync(user);

                // Atualizar último login
                await UpdateLastLoginAsync(user.Id);

                // Obter roles do usuário
                var roles = await GetUserRolesAsync(user.Id);

                _logger.LogInformation("Autenticação bem-sucedida para usuário: {Username}", username);

                return new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    TenantId = user.TenantId,
                    Roles = roles,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante autenticação do usuário: {Username}", username);
                return null;
            }
        }

        /// <summary>
        /// Obtém um usuário pelo ID
        /// </summary>
        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);

                if (user == null)
                {
                    return null;
                }

                var roles = await GetUserRolesAsync(userId);

                return new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    TenantId = user.TenantId,
                    Roles = roles,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário por ID: {UserId}", userId);
                return null;
            }
        }

        /// <summary>
        /// Obtém um usuário pelo username
        /// </summary>
        public async Task<UserDto?> GetUserByUsernameAsync(string username, int? tenantId = null)
        {
            try
            {
                var user = await GetUserEntityByUsernameAsync(username, tenantId);

                if (user == null)
                {
                    return null;
                }

                var roles = await GetUserRolesAsync(user.Id);

                return new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    TenantId = user.TenantId,
                    Roles = roles,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário por username: {Username}", username);
                return null;
            }
        }

        /// <summary>
        /// Verifica se um usuário tem uma role específica
        /// </summary>
        public async Task<bool> UserHasRoleAsync(int userId, string role)
        {
            try
            {
                var userRoles = await GetUserRolesAsync(userId);
                return userRoles.Contains(role, StringComparer.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar role do usuário: {UserId}, Role: {Role}", userId, role);
                return false;
            }
        }

        /// <summary>
        /// Obtém todas as roles de um usuário
        /// </summary>
        public async Task<List<string>> GetUserRolesAsync(int userId)
        {
            try
            {
                var userRoles = await _userRoleRepository.GetUserRolesAsync(userId);
                var roleNames = new List<string>();

                foreach (var userRole in userRoles)
                {
                    var role = await _roleRepository.GetByIdAsync(userRole.RoleId);
                    if (role != null)
                    {
                        roleNames.Add(role.Name);
                    }
                }

                return roleNames;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter roles do usuário: {UserId}", userId);
                return new List<string>();
            }
        }

        /// <summary>
        /// Verifica se um usuário está ativo
        /// </summary>
        public async Task<bool> IsUserActiveAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                return user?.IsActive ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar se usuário está ativo: {UserId}", userId);
                return false;
            }
        }

        /// <summary>
        /// Atualiza a data do último login do usuário
        /// </summary>
        public async Task UpdateLastLoginAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    user.LastLoginAt = DateTime.UtcNow;
                    user.UpdatedAt = DateTime.UtcNow;
                    await _userRepository.UpdateAsync(user);

                    _logger.LogInformation("Último login atualizado para usuário: {UserId}", userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar último login do usuário: {UserId}", userId);
            }
        }

        /// <summary>
        /// Valida se a senha está correta para um usuário
        /// </summary>
        public async Task<bool> ValidatePasswordAsync(int userId, string password)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return false;
                }

                return VerifyPassword(password, user.PasswordHash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar senha do usuário: {UserId}", userId);
                return false;
            }
        }

        /// <summary>
        /// Cria um hash da senha usando BCrypt
        /// </summary>
        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Senha não pode ser vazia", nameof(password));
            }

            // Usar work factor configurável (padrão: 12)
            var workFactor = _configuration.GetValue<int>("Authentication:BCryptWorkFactor", 12);
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor);
        }

        /// <summary>
        /// Verifica se uma senha corresponde ao hash usando BCrypt
        /// </summary>
        public bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
            {
                return false;
            }

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar senha");
                return false;
            }
        }

        #region Métodos Privados

        /// <summary>
        /// Busca usuário por username ou email considerando tenant
        /// </summary>
        private async Task<User?> GetUserEntityByUsernameAsync(string username, int? tenantId = null)
        {
            // Primeiro tenta buscar por username
            var user = await _userRepository.GetByUsernameAsync(username, tenantId);

            // Se não encontrar, tenta buscar por email
            if (user == null && IsValidEmail(username))
            {
                user = await _userRepository.GetByEmailAsync(username, tenantId);
            }

            return user;
        }

        /// <summary>
        /// Incrementa as tentativas de login falhadas e bloqueia conta se necessário
        /// </summary>
        private async Task IncrementFailedLoginAttemptsAsync(User user)
        {
            try
            {
                user.FailedLoginAttempts++;
                user.UpdatedAt = DateTime.UtcNow;

                var maxFailedAttempts = _configuration.GetValue<int>("Authentication:MaxFailedAccessAttempts", 5);
                var lockoutTimeSpan = _configuration.GetValue<TimeSpan>("Authentication:LockoutTimeSpan", TimeSpan.FromMinutes(5));

                if (user.FailedLoginAttempts >= maxFailedAttempts)
                {
                    user.LockoutEnd = DateTime.UtcNow.Add(lockoutTimeSpan);
                    _logger.LogWarning("Conta bloqueada por excesso de tentativas: {Username}", user.Username);
                }

                await _userRepository.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao incrementar tentativas de login falhadas para usuário: {UserId}", user.Id);
            }
        }

        /// <summary>
        /// Reseta as tentativas de login falhadas
        /// </summary>
        private async Task ResetFailedLoginAttemptsAsync(User user)
        {
            try
            {
                if (user.FailedLoginAttempts > 0 || user.LockoutEnd.HasValue)
                {
                    user.FailedLoginAttempts = 0;
                    user.LockoutEnd = null;
                    user.UpdatedAt = DateTime.UtcNow;
                    await _userRepository.UpdateAsync(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao resetar tentativas de login falhadas para usuário: {UserId}", user.Id);
            }
        }

        /// <summary>
        /// Valida se uma string é um email válido
        /// </summary>
        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Métodos Adicionais para Gerenciamento de Usuários

        /// <summary>
        /// Cria um novo usuário
        /// </summary>
        public async Task<UserDto?> CreateUserAsync(string username, string email, string password, string? fullName = null, int? tenantId = null)
        {
            try
            {
                // Verificar se username já existe
                var existingUser = await GetUserEntityByUsernameAsync(username, tenantId);
                if (existingUser != null)
                {
                    _logger.LogWarning("Tentativa de criar usuário com username já existente: {Username}", username);
                    return null;
                }

                // Verificar se email já existe
                if (!string.IsNullOrEmpty(email))
                {
                    var existingEmailUser = await _userRepository.GetByEmailAsync(email, tenantId);
                    if (existingEmailUser != null)
                    {
                        _logger.LogWarning("Tentativa de criar usuário com email já existente: {Email}", email);
                        return null;
                    }
                }

                var user = new User
                {
                    Username = username,
                    Email = email,
                    FullName = fullName,
                    PasswordHash = HashPassword(password),
                    TenantId = tenantId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    FailedLoginAttempts = 0
                };

                var createdUser = await _userRepository.CreateAsync(user);

                _logger.LogInformation("Usuário criado com sucesso: {Username}", username);

                return new UserDto
                {
                    Id = createdUser.Id,
                    Username = createdUser.Username,
                    Email = createdUser.Email,
                    FullName = createdUser.FullName,
                    TenantId = createdUser.TenantId,
                    Roles = new List<string>(),
                    IsActive = createdUser.IsActive,
                    CreatedAt = createdUser.CreatedAt,
                    UpdatedAt = createdUser.UpdatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar usuário: {Username}", username);
                return null;
            }
        }

        /// <summary>
        /// Altera a senha de um usuário
        /// </summary>
        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return false;
                }

                // Verificar senha atual
                if (!VerifyPassword(currentPassword, user.PasswordHash))
                {
                    _logger.LogWarning("Tentativa de alteração de senha com senha atual incorreta: {UserId}", userId);
                    return false;
                }

                // Atualizar senha
                user.PasswordHash = HashPassword(newPassword);
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);

                _logger.LogInformation("Senha alterada com sucesso para usuário: {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar senha do usuário: {UserId}", userId);
                return false;
            }
        }

        /// <summary>
        /// Ativa ou desativa um usuário
        /// </summary>
        public async Task<bool> SetUserActiveStatusAsync(int userId, bool isActive)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return false;
                }

                user.IsActive = isActive;
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);

                _logger.LogInformation("Status do usuário alterado: {UserId}, Ativo: {IsActive}", userId, isActive);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar status do usuário: {UserId}", userId);
                return false;
            }
        }

        #endregion
    }
}

