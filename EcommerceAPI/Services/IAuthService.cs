using EcommerceAPI.DTOs;
using EcommerceAPI.Models;
using System.Threading.Tasks;

namespace EcommerceAPI.Services
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginModel loginModel);
        Task<bool> RegisterUserAsync(RegisterUserDTO model);
    }
}
