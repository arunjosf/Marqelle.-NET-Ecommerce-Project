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
    public class UsersController : ControllerBase
    {
        private readonly IUserServices _userService;
        private readonly IJwtService _jwtService;
        private readonly IPasswordService _passwordService;

        public UsersController(IUserServices userService, IJwtService jwtService, IPasswordService passwordService)
        {
            _userService = userService;
            _jwtService = jwtService;
            _passwordService = passwordService;
        }

        [HttpPost("register")]
        public ActionResult Register([FromForm] RegisterRequestDto dto) 
        {
            var createdUser = _userService.Register(dto);
            return Ok("registrd"); 
        }


        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginRequestDto request)
        {
            var user = _userService.Login(request.Email, request.Password);
            if (user == null) return Unauthorized("Invalid credentials");

            var accessToken = _jwtService.GenerateToken(user);

            var refreshToken = RefreshToken.GenerateRefreshToken();

            var hashedToken = _passwordService.Hash(refreshToken, user);
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7); 
            _userService.UpdateRefreshToken(user.Id, hashedToken, refreshTokenExpiry);

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

            return Ok(new
            {
                Token = accessToken,
                refreshToken

            });
        }

        [Authorize]
        [HttpPost("logout")]
        public ActionResult LogOut()
        {
            var UserIdclaim = User.FindFirst("UserId");

            if(UserIdclaim == null)
            {
                return Unauthorized();
            }

            var userId = long.Parse(UserIdclaim.Value);
            _userService.LogOut(userId);
            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");
            return Ok("Logged Out Succefully");
        }

        [HttpPost("refresh")]
        public ActionResult Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"] ?? "";
            refreshToken = Uri.UnescapeDataString(refreshToken);


            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();

            var user = _userService.ValidateRefreshToken(refreshToken);

            if (user == null)
            { 
                return BadRequest("2");
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

            return Ok(new
            {
                Token = newAccessToken
            });
        }
    }
}
     
