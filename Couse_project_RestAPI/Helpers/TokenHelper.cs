using Couse_project_RestAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

using Couse_project_RestAPI.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Couse_project_RestAPI.Helpers
{
    public class TokenHelper
    {
        private IConfiguration _configuration;

        public TokenHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }



        public JwtSecurityToken CreateToken(User user)
        {
            // Формируем claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("roleId", user.Id_role.ToString()),
                new Claim("role", user.Role?.Name ?? "User"),
                new Claim("isActive", user.Is_active.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // уникальный ID токена
            };

            // Создаём ключ и подпись
            IConfigurationSection jwtSettings = _configuration.GetSection("JwtSettings");
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Генерируем и возвращаем токен
            return new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["AccessTokenExpiryMinutes"])),
                signingCredentials: creds
            );
        }
    }
}
