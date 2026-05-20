using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Transport.Application.Interfaces;
using Transport.Domain.Entities;

namespace Transport.Infrastructure.Security
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user, string roleName)
        {
            var jwtKey = GetRequiredJwtSetting("Jwt:Key");
            var issuer = GetRequiredJwtSetting("Jwt:Issuer");
            var audience = GetRequiredJwtSetting("Jwt:Audience");
            var expiresInMinutes = int.TryParse(_configuration["Jwt:ExpiresInMinutes"], out var value) ? value : 60;

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, user.Username),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Role, roleName)
            };

            claims.AddRange(user.Role.RolePermissions
                .Select(rolePermission => new Claim("permission", rolePermission.Permission.Name)));

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            {
                KeyId = "TransportStudentSystemJwtKey"
            };
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GetRequiredJwtSetting(string key)
        {
            var value = _configuration[key]?.Trim();

            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException($"{key} is not configured");

            if (key == "Jwt:Key" && Encoding.UTF8.GetByteCount(value) < 32)
                throw new InvalidOperationException("Jwt:Key must be at least 32 bytes for HMAC SHA256");

            return value;
        }
    }
}
