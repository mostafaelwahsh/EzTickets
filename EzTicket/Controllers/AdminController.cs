using AutoMapper;
using EzTickets.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        private readonly Mapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager, Mapper mapper,
                               RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        [HttpGet("users")]
        public Task<ActionResult<PagedResponse<GeneralResponse>>> GetUsers(PaginationParams pagination)
        {

        }
    }
}
