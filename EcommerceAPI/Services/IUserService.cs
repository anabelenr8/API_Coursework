using EcommerceAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceAPI.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetUsersAsync();
        Task<bool> RegisterUserAsync(RegisterUserDTO model);
        Task<bool> UpdateUserAsync(int id, UserDTO updatedUser);
        Task<bool> DeleteUserAsync(int id);
    }
}
