using EcommerceAPI.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EcommerceAPI.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetUsersAsync();
        Task<bool> UpdateUserAsync(string id, UserDTO updatedUser);
        Task<bool> DeleteUserAsync(string id);
        Task<bool> UpdateUserRoleAsync(Guid id, string newRole);

    }
}
