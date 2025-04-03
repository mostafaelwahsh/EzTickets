using EzTickets.DTO;
using Models;

namespace EzTickets.Repository
{
    public interface IUserRepository
    {
        Task<PagedResponse<AdminUserDTO>> GetUsersAsync(PaginationParams pagination);
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<bool> AssignRoleAsync(string userId, string roleName);
    }
}
