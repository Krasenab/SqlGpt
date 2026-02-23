using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SqlGpt.Models;
using SqlGpt.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SqlGpt.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;
        public JwtService(IConfiguration configuration) 
        {
            this._config = configuration;
        }
        public string GenerateToken(AppUser user)
        {
            var jwt = _config.GetSection("Jwt");

            var issuer = jwt["Issuer"];
            var audience = jwt["Audience"];
            var key = jwt["Key"];
            var expiresMinutes = int.Parse(jwt["ExpiresInMinutes"] ?? "60");

            var claims = new List<Claim>
             {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));

            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
