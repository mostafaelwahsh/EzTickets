using AutoMapper;
using AutoMapper.QueryableExtensions;
using EzTickets.DTO.Admin;
using EzTickets.DTO.Pagination;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;

namespace EzTickets.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UserRepository(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<PagedResponse<AdminUserDTO>> GetUsersAsync (PaginationParams pagination)
        {
            var query = _userManager.Users.OrderBy(u => u.Email);

            var pagedData = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ProjectTo<AdminUserDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var totalRecords = await query.CountAsync();

            return new PagedResponse<AdminUserDTO>(
                pagedData,
                pagination.PageNumber,
                pagination.PageSize,
                totalRecords);
        }

        public async Task<ApplicationUser?> GetUserByIdAsync (string userId)
        {
           var user = await _userManager.FindByIdAsync(userId);
           return user;
        }
        public async Task<bool> AssignRoleAsync(string userId, string roleName)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.AddToRoleAsync(user, roleName);
            return result.Succeeded;
        }
    }
}
