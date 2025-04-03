using EzTickets.DTO.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace EzTickets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("my-profile")]
        public async Task<ActionResult<GeneralResponse>> MyProfile()
        {
            GeneralResponse response = new GeneralResponse();
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                return Ok(new GeneralResponse
                {
                    IsPass = true,
                    Data = new UserProfileDTO
                    {
                        FullName = user.FullName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        BloodType = user.BloodType,
                        City = user.City,
                    }
                });
            }
            return Unauthorized();
        }

        [HttpPatch("my-profile")]
        public async Task<ActionResult<GeneralResponse>> UpdateProfile(UpdateProfileDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new GeneralResponse
                {
                    IsPass = false,
                    Data = ModelState.Values.SelectMany(v => v.Errors)
                });
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(request.FullName))
                    user.FullName = request.FullName;

                if (!string.IsNullOrEmpty(request.PhoneNumber))
                    user.PhoneNumber = request.PhoneNumber;

                if (!string.IsNullOrEmpty(request.BloodType))
                    user.BloodType = request.BloodType;

                if (!string.IsNullOrEmpty(request.City))
                    user.City = request.City;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    return BadRequest(new GeneralResponse
                    {
                        IsPass = false,
                        Data = result.Errors
                    });

                UserProfileDTO userProfile = new UserProfileDTO()
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    BloodType = user.BloodType,
                    City = user.City,
                };
                return Ok(new GeneralResponse { IsPass = true });
            }

            return Unauthorized();
        }
        [HttpPost("my-profile/change-password")]
        public async Task<ActionResult<GeneralResponse>> ChangePassword(ChangePasswordDTO request)
        {
            GeneralResponse response = new GeneralResponse();

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(
                    user,
                    request.CurrentPassword,
                    request.NewPassword);

                if (!result.Succeeded)
                {
                    response.IsPass = false;
                    response.Data = result.Errors;
                    return response;
                }
                response.IsPass = true;
                response.Data = "Password changed successfully";
                return Ok(new GeneralResponse { IsPass = true });
            }
            return Unauthorized();
        }
    }
}
