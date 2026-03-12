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
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;
        private readonly IUserAuthServices _userService; 

        public CheckoutController(ICheckoutService checkoutService, IUserAuthServices userService)
        {
            _checkoutService = checkoutService;
            _userService = userService;
        }

        [HttpGet("checkout-page")]
        public async Task<IActionResult> GetCheckoutPage()
        {
            var userId = GetUserIdFromCookie();

            if (userId == 0)
                return Unauthorized(new ApiResponseDto<object>(
                    StatusCodes.Status401Unauthorized,
                    false,
                    "User not logged in",
                    null
                ));

            var checkoutData = await _checkoutService.GetCheckoutPageAsync(userId);

            return Ok(new ApiResponseDto<object>(
                 StatusCodes.Status200OK,
                 true,
                 "Checkout data fetched successfully",
                 checkoutData
             ));
        }

        private long GetUserIdFromCookie()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (userIdClaim == null)
                throw new Exception("User ID not found in token.");

            return Convert.ToInt64(userIdClaim);
        }
    }
}
