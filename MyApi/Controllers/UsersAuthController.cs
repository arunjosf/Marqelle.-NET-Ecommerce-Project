using Marqelle.Application.DTO;
using Marqelle.Application.Helpers;
using Marqelle.Application.Interfaces;
using Marqelle.Domain.Entities;
using Marqelle.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marqelle.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    public class UsersAuthController : ControllerBase
    {
        private readonly IUserAuthServices _userService;
        private readonly IJwtService _jwtService;
        private readonly IPasswordService _passwordService;

        public UsersAuthController(IUserAuthServices userService, IJwtService jwtService, IPasswordService passwordService)
        {
            _userService = userService;
            _jwtService = jwtService;
            _passwordService = passwordService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromForm] RegisterRequestDto dto) 
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new
                    {
                        Field = x.Key,
                        Message = x.Value.Errors.First().ErrorMessage
                    }).ToList();

                return BadRequest(new ApiResponseDto<object>(
                    StatusCodes.Status400BadRequest,
                    false,
                    "Validation Failed",
                    errors
                ));
            }

            var createdUser = await _userService.Register(dto);

            return Ok(new ApiResponseDto<object>(
               StatusCodes.Status200OK,
               true,
               "User registered successfully",
               createdUser
           ));
        }


        [HttpPost("login")]
        public async Task<ActionResult> Login([FromForm] LoginRequestDto request)
        {
            var user = await _userService.Login(request.Email, request.Password);
            if (user == null)
            {
                return Unauthorized(new ApiResponseDto<object>(
                    StatusCodes.Status401Unauthorized,
                    false,
                    "Invalid email or password",
                    null
                ));
            }

            var accessToken = _jwtService.GenerateToken(user);

            var refreshToken = RefreshToken.GenerateRefreshToken();

            var hashedToken = _passwordService.Hash(refreshToken, user);
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userService.UpdateRefreshToken(user.Id, hashedToken, refreshTokenExpiry);

            var accessCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddMinutes(60),
                Secure = true,
                SameSite = SameSiteMode.Strict
            };
            Response.Cookies.Append("accessToken", accessToken, accessCookieOptions);

            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshTokenExpiry,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };
            Response.Cookies.Append("refreshToken", refreshToken, refreshCookieOptions);

            return Ok(new ApiResponseDto<object>(
                StatusCodes.Status200OK,
                true,
                "Login successful",
                 new {
                     Token = accessToken,
                     RefreshToken = refreshToken
                 }
                ));
        }
        

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult> LogOut()
        {
            var UserIdclaim = User.FindFirst("UserId");

            if (UserIdclaim == null)
            {
                return Unauthorized(new ApiResponseDto<object>(
                    StatusCodes.Status401Unauthorized,
                    false,
                    "User not authenticated",
                    null
                ));
            }

            var userId = long.Parse(UserIdclaim.Value);
           await _userService.LogOut(userId);
            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");

            return Ok(new ApiResponseDto<object>(
            StatusCodes.Status200OK,
            true,
            "Logged out successfully",
            null
     ));
        }

        [HttpPost("refresh")]
        public async Task<ActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"] ?? "";
            refreshToken = Uri.UnescapeDataString(refreshToken);

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new ApiResponseDto<object>(
                    StatusCodes.Status401Unauthorized,
                    false,
                    "Refresh token not found",
                    null
                ));
            }

            var user = await _userService.ValidateRefreshToken(refreshToken);

            if (user == null)
            {
                return BadRequest(new ApiResponseDto<object>(
                    StatusCodes.Status400BadRequest,
                    false,
                    "Invalid or expired refresh token",
                    null
                ));
            }


            var newAccessToken = _jwtService.GenerateToken(user);

            var accessCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddMinutes(60),
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("accessToken", newAccessToken, accessCookieOptions);

            return Ok(new ApiResponseDto<object>(
            StatusCodes.Status200OK,
            true,
            "Access token refreshed successfully",
            new {

            Token = newAccessToken
        }
    ));
        }
    }
}
     
