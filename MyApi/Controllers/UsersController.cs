using Marqelle.Application.DTO;
using Marqelle.Api.Services;
using Marqelle.Domain.Entities;
using Marqelle.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Marqelle.Application.Helpers;

using Microsoft.AspNetCore.Mvc;

namespace Marqelle.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserServices _userService;
        private readonly JwtService _jwtService;

        public UsersController(IUserServices userService, JwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public ActionResult Register([FromBody] RegisterRequestDto dto)
        {
            var createdUser = _userService.Register(dto);
            return Ok("registrd");
        }


        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginRequestDto request)
        {
            // 1️⃣ Validate user credentials
            var user = _userService.Login(request.Email, request.Password);
            if (user == null) return Unauthorized("Invalid credentials");

            // 2️⃣ Generate access token (JWT)
            var accessToken = _jwtService.GenerateToken(user);

            // 3️⃣ Generate a new refresh token
            var refreshToken = RefreshTokenHasher.GenerateRefreshToken();

            // 4️⃣ Hash the refresh token and store in DB with expiry
            var hashedToken = PasswordHasher.HashPassword(refreshToken);
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7); // 7 days expiry
            _userService.UpdateRefreshToken(user.Id, hashedToken, refreshTokenExpiry);

            // 5️⃣ Set access token in HttpOnly cookie
            var accessCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddMinutes(60), // match JWT expiry
                Secure = true,
                SameSite = SameSiteMode.Strict
            };
            Response.Cookies.Append("accessToken", accessToken, accessCookieOptions);

            // 6️⃣ Set refresh token in HttpOnly cookie
            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshTokenExpiry,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };
            Response.Cookies.Append("refreshToken", refreshToken, refreshCookieOptions);

            // 7️⃣ Return access token in response body (optional)
            return Ok(new
            {
                Token = accessToken,
                refreshToken

            });
        }
    }
}
    
