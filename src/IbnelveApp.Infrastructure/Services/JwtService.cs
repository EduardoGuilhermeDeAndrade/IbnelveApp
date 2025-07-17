using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IbnelveApp.Application.DTOs.Auth;
using IbnelveApp.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace IbnelveApp.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationMinutes;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
            _secretKey = _configuration["JwtSettings:SecretKey"] ?? throw new ArgumentNullException("JwtSettings:SecretKey não configurado");
            _issuer = _configuration["JwtSettings:Issuer"] ?? "IbnelveApp";
            _audience = _configuration["JwtSettings:Audience"] ?? "IbnelveApp";
            _expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "60");
        }

        public string GenerateToken(UserDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("tenant_id", user.TenantId?.ToString() ?? "")
            };

            // Adicionar roles como claims
            if (user.Roles != null && user.Roles.Any())
            {
                foreach (var role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_expirationMinutes),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public string? GetUserIdFromToken(string token)
        {
            var principal = ValidateToken(token);
            return principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public string? GetUsernameFromToken(string token)
        {
            var principal = ValidateToken(token);
            return principal?.FindFirst(ClaimTypes.Name)?.Value;
        }

        public List<string> GetRolesFromToken(string token)
        {
            var principal = ValidateToken(token);
            return principal?.FindAll(ClaimTypes.Role)?.Select(c => c.Value).ToList() ?? new List<string>();
        }

        public string? GetTenantIdFromToken(string token)
        {
            var principal = ValidateToken(token);
            return principal?.FindFirst("tenant_id")?.Value;
        }

        public bool IsTokenExpired(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadJwtToken(token);
                return jsonToken.ValidTo < DateTime.UtcNow;
            }
            catch
            {
                return true;
            }
        }

        public DateTime? GetTokenExpirationDate(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadJwtToken(token);
                return jsonToken.ValidTo;
            }
            catch
            {
                return null;
            }
        }
    }
}

