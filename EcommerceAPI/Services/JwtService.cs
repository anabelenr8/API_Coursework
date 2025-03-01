using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EcommerceAPI.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(ClaimsIdentity identity, string userId, string role)
        {
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? _configuration["JwtSettings:SecretKey"]
                ?? throw new InvalidOperationException("JWT Secret Key is missing!");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));


            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddHours(2);

            var token = new JwtSecurityToken(
                _configuration["JwtSettings:Issuer"],
                _configuration["JwtSettings:Audience"],
                identity.Claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:CNjJtE01Vry7XZuARdIaFPoJtogF+okPgJQKSyPq9+g="] ?? "YOUR_DEFAULT_SECRET_KEY");

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return principal;
        }
    }
}
