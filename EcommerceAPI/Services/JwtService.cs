using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace EcommerceAPI.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly string _jwtSecret;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
                         ?? _configuration["JwtSettings:SecretKey"]
                         ?? throw new InvalidOperationException("JWT Secret Key is missing!");

            _issuer = _configuration["JwtSettings:Issuer"]
                      ?? throw new InvalidOperationException("JWT Issuer is missing!");

            _audience = _configuration["JwtSettings:Audience"]
                        ?? throw new InvalidOperationException("JWT Audience is missing!");

            // Debugging: Verify secret is loaded correctly
            Console.WriteLine($"🔍 JWT_SECRET Loaded in JwtService: {_jwtSecret}");
        }

        public string GenerateJwtToken(ClaimsIdentity identity, string userId, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = _jwtSecret ?? throw new Exception("JWT Secret not found!");
            var key = Encoding.ASCII.GetBytes(secretKey);



            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.UtcNow.AddMinutes(30), // ✅ Adjust expiry as needed
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var secretKey = _jwtSecret ?? throw new Exception("JWT Secret not found!");
            var key = Encoding.ASCII.GetBytes(secretKey);


            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false, // Allow expired tokens to be validated
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return principal;
        }
    }
}
