using System.Security.Claims;

namespace EcommerceAPI.Services
{
    public interface IJwtService
    {
        string GenerateJwtToken(ClaimsIdentity identity, string userId, string role);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
