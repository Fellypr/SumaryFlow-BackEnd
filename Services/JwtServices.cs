using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SumaryYoutubeBackend.Models;

namespace SumaryYoutubeBackend.Services
{
    public class JwtServices : IJwtServices
    {
        private readonly IConfiguration _configuration;

        private const string DefaultKey = "CARACTERESPECIALPARAJWT1234567890";
        private const string DefaultIssuer = "SumaryYoutubeBackend";
        private const string DefaultAudience = "SumaryYoutubeFrontend";

        public JwtServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateTokenAuth(AuthUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var key = _configuration["Jwt:Key"] ?? DefaultKey;
            var issuer = _configuration["Jwt:Issuer"] ?? DefaultIssuer;
            var audience = _configuration["Jwt:Audience"] ?? DefaultAudience;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

