using AutoMapper;
using EzTickets.DTO.Admin;
using EzTickets.DTO.Pagination;
using EzTickets.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace EzTickets.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserRepository _userRepository;
        private readonly IInfoRepository _infoRepository;
        private readonly IEventRepository _eventRepository;

        public AdminController(UserManager<ApplicationUser> userManager, IMapper mapper,
                               RoleManager<IdentityRole> roleManager, IUserRepository userRepository,
                               IInfoRepository infoRepository, IEventRepository eventRepository)
        {
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
            _userRepository = userRepository;
            _infoRepository = infoRepository;
            _eventRepository = eventRepository;
        }

        [HttpGet("contact-requests")]
        public async Task<ActionResult<GeneralResponse>> GetContactRequests()
        {
            var requests = await _infoRepository.GetAllRequests();
            var contactRequests = _mapper.Map<List<ContactRequestDTO>>(requests);
            return (new GeneralResponse
            {
                IsPass = true,
                Data = requests
            });
        }

        [HttpGet("statistics")]
        public ActionResult<GeneralResponse> GetStatistics([FromQuery] PaginationParams pagination)
        {
            try
            {
                var pagedEvents = _eventRepository.GetAllPublic(pagination);
                var result = new PagedResponse<SalesReportDTO>(
                    _mapper.Map<List<SalesReportDTO>>(pagedEvents.Data),
                    pagedEvents.PageNumber,
                    pagedEvents.PageSize,
                    pagedEvents.TotalRecords);

                return new GeneralResponse
                {
                    IsPass = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse
                {
                    IsPass = false,
                };
            }
        }

        [HttpGet("users")]
        public async Task<ActionResult<GeneralResponse>> GetUsers([FromQuery] PaginationParams pagination)
        {
            try
            {
                var users = await _userRepository.GetUsersAsync(pagination);
                return (new GeneralResponse
                {
                    IsPass = true,
                    Data = users
                });
            }
            catch (Exception ex)
            {
                return (new GeneralResponse
                {
                    IsPass = false,
                    Data = ex.Message
                });
            }
        }

        [HttpGet("users/{userId}")]
        public async Task<ActionResult<GeneralResponse>> GetUserById(string userId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);

                if (user == null)
                    return NotFound(new GeneralResponse
                    {
                        IsPass = false,
                        Data = "User not found"
                    });

                var userDto = _mapper.Map<AdminUserDetailsDTO>(user);

                var roles = await _userManager.GetRolesAsync(user);
                userDto.Roles = roles.ToList();

                return Ok(new GeneralResponse
                {
                    IsPass = true,
                    Data = userDto
                });
            }
            catch (Exception ex)
            {
                return (new GeneralResponse
                {
                    IsPass = false,
                    Data = "An error occurred while fetching user"
                });
            }
        }
        [HttpPatch("users/{userId}/roles")]
        public async Task<ActionResult<GeneralResponse>> AssignRole(
        string userId, RoleUpdateDTO roleUpdate)
        {
            try
            {

                if (!ModelState.IsValid)
                    return (new GeneralResponse
                    {
                        IsPass = false,
                        Data = ModelState
                    });

                var result = await _userRepository.AssignRoleAsync(userId, roleUpdate.RoleName);

                if (!result)
                    return (new GeneralResponse
                    {
                        IsPass = false,
                        Data = "Failed to assign role"
                    });

                return (new GeneralResponse
                {
                    IsPass = true,
                    Data = "Role assigned successfully"
                });
            }
            catch (Exception ex)
            {
                return (new GeneralResponse
                {
                    IsPass = false,
                    Data = "An error occurred while assigning role"
                });
            }
        }

    }
}
