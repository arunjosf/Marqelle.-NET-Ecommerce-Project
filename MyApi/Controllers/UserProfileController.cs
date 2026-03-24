using Marqelle.Application.DTO;
using Marqelle.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marqelle.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserAuthServices _userService;

        public UserProfileController(IUserAuthServices userService)
        {
            _userService = userService;
        }

        [HttpPut("change-password")]
        public async Task<ActionResult> ChangePassword(
            [FromQuery] string currentPassword,
            [FromQuery] string newPassword)
        {
            var userId = GetUserId();
            try
            {
                await _userService.ChangePasswordAsync(userId, currentPassword, newPassword);
                return Ok(new ApiResponseDto<object>(StatusCodes.Status200OK, true, "Password changed successfully.", null));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<object>(StatusCodes.Status400BadRequest, false, ex.Message, null));
            }
        }

        [HttpPut("update-profile")]
        public async Task<ActionResult> UpdateProfile(
            [FromQuery] string firstName,
            [FromQuery] string lastName,
            [FromQuery] string email)
        {
            var userId = GetUserId();
            try
            {
                await _userService.UpdateProfileAsync(userId, firstName, lastName, email);
                return Ok(new ApiResponseDto<object>(StatusCodes.Status200OK, true, "Profile updated successfully.", null));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<object>(StatusCodes.Status400BadRequest, false, ex.Message, null));
            }
        }

        [HttpPost("change-email/send-otp")]
        public async Task<ActionResult> SendChangeEmailOtp([FromQuery] string newEmail)
        {
            var userId = GetUserId();
            try
            {
                await _userService.SendChangeEmailOtpAsync(userId, newEmail);
                return Ok(new ApiResponseDto<object>(StatusCodes.Status200OK, true, "OTP sent to your new email address.", null));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<object>(StatusCodes.Status400BadRequest, false, ex.Message, null));
            }
        }

        [HttpPost("change-email/verify")]
        public async Task<ActionResult> VerifyAndChangeEmail([FromQuery] string otpCode)
        {
            try
            {
                await _userService.VerifyAndChangeEmailAsync(otpCode);
                return Ok(new ApiResponseDto<object>(StatusCodes.Status200OK, true, "Email updated successfully.", null));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto<object>(StatusCodes.Status400BadRequest, false, ex.Message, null));
            }
        }

        [HttpGet("userprofile")]
        public async Task<ActionResult> GetProfile()
        {
            var userId = GetUserId();
            var user = await _userService.GetProfileAsync(userId);
            return Ok(new ApiResponseDto<object>(StatusCodes.Status200OK, true, "Profile fetched.", user));
        }

        private long GetUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null)
                throw new Exception("User ID not found in token.");
            return Convert.ToInt64(userIdClaim);
        }
    }
}