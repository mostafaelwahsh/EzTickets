using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EzTickets.DTO.Public;
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

        public AccountController(UserManager<ApplicationUser> userManager, IConfiguration config,
               RoleManager<IdentityRole> roleManager)
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
                    if (user.UserName == "admin@admin.com")
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                    }
                    
                    return (new GeneralResponse()
                    {
                        IsPass = true,
                        Data = "User Registered Successfully"
                    });
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
                    if(exist == true)
                    {
                        List<Claim> claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                        claims.Add(new Claim(ClaimTypes.Name, user.FullName));
                        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

                        var Roles = await _userManager.GetRolesAsync(user);
                        foreach (var RoleName in Roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role,RoleName));
                        }

                        string key = _config["JWT:Key"];
                        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                        SigningCredentials signingCredentials =
                            new SigningCredentials(secretKey,SecurityAlgorithms.HmacSha256);

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
                return Unauthorized();
            }

            response.IsPass = false;
            response.Data=ModelState;
            return response;
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<GeneralResponse>> Logout()
        {
            return Ok(new GeneralResponse
            {
                IsPass = true,
                Data = "Logged out successfully. Please discard your token."
            });
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<GeneralResponse>> ForgotPassword(ForgotPasswordDTO model)
        {
            if (!ModelState.IsValid)
                return new GeneralResponse { IsPass = false, Data = ModelState };

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new GeneralResponse { IsPass = true, Data = "If your email exists, you'll receive a password reset link" };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            
            // we are returning it directly (only for development)
            var resetLink = Url.Action("ResetPassword", "Account",
                new { email = model.Email, token = token }, Request.Scheme);

            return new GeneralResponse
            {
                IsPass = true,
                Data = new
                {
                    Message = "Password reset link generated",
                    ResetLink = resetLink 
                }
            };
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<GeneralResponse>> ResetPassword(ResetPasswordDTO fromRequest)
        {
            if (!ModelState.IsValid)
                return new GeneralResponse { IsPass = false, Data = ModelState };

            var user = await _userManager.FindByEmailAsync(fromRequest.Email);
            if (user == null)
                return new GeneralResponse { IsPass = true, Data = "Password reset successful" };

            var result = await _userManager.ResetPasswordAsync(user, fromRequest.Token, fromRequest.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return new GeneralResponse { IsPass = false, Data = ModelState };
            }

            return new GeneralResponse { IsPass = true, Data = "Password has been reset successfully" };
        }
    }
}
