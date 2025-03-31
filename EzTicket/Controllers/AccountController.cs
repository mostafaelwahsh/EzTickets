using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EzTickets.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models;

namespace EzTickets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public AccountController(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<ActionResult<GeneralResponse>> Register(RegisterUserDTO userFromRequest)
        {
            GeneralResponse response = new GeneralResponse();

            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    Email = userFromRequest.Email,
                    UserName = userFromRequest.Email,
                    FullName = userFromRequest.FullName,
                    PhoneNumber = userFromRequest.PhoneNumber
                };
                IdentityResult result = await _userManager.CreateAsync(user, userFromRequest.Password);
                if (result.Succeeded)
                {
                    if (user.Email is "admin@admin.com")
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                    await _userManager.AddToRoleAsync(user, "User");
                    response.IsPass = true;
                    response.Data = "User Registered Successfully";
                    return response;
                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            response.IsPass = false;
            response.Data = ModelState;
            return response;

        }

        [HttpPost("login")]
        public async Task<ActionResult<GeneralResponse>> Login(LoginUserDTO userFromRequest)
        {
            GeneralResponse response = new GeneralResponse();

            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(userFromRequest.Email);
                if (user != null)
                {
                    bool exist = await _userManager.CheckPasswordAsync(user, userFromRequest.Password);
                    if (exist == true)
                    {
                        List<Claim> claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                        claims.Add(new Claim(ClaimTypes.Name, user.FullName));
                        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

                        var Roles = await _userManager.GetRolesAsync(user);
                        foreach (var RoleName in Roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, RoleName));
                        }

                        string key = _config["JWT:Key"];
                        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                        SigningCredentials signingCredentials =
                            new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                        JwtSecurityToken jwtSecurityToken = new JwtSecurityToken
                            (
                            issuer: _config["JWT:Issuer"],
                            audience: _config["JWT:Audience"],
                            expires: DateTime.Now.AddHours(1),
                            claims: claims,
                            signingCredentials: signingCredentials
                            );

                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                            expires = DateTime.Now.AddHours(1)
                        });
                    }
                }
                return Unauthorized(new GeneralResponse
                {
                    IsPass = false,
                    Data = "Invalid email or password"
                });
            }

            response.IsPass = false;
            response.Data = ModelState;
            return response;
        }

        [Authorize]
        [HttpPost("logout")]
        public ActionResult<GeneralResponse> Logout()
        {
            return (new GeneralResponse() { IsPass = true, Data = "Please Discard The Token" });
        }
    }
}
