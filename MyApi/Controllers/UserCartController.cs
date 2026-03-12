using Marqelle.Application.DTO;
using Marqelle.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Marqelle.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserCartController : ControllerBase
    {
        private readonly IUserCartService _service;
        public UserCartController(IUserCartService service)
        {
            _service = service;
        }

        private long GetUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (userIdClaim == null)
                throw new Exception("User ID not found in token.");

            return Convert.ToInt64(userIdClaim);
        }

        [HttpPost("add")]
        public async Task<ActionResult> AddToCart(long productId, string size)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto<object>(
                    StatusCodes.Status401Unauthorized,
                    false,
                    "User not authenticated",
                    null
                ));
            }

            var result = await _service
                .AddToCart(userId, productId, size);

            if(result == null)
            {
                return BadRequest(new ApiResponseDto<object>(
                    StatusCodes.Status400BadRequest,
                    false,
                    "Item already in cart",
                    null
                    ));
            }

            return Ok(new ApiResponseDto<object>(
                StatusCodes.Status200OK,
                true,
                "Product added to cart",
                result
            ));
        }

        [HttpGet("Cartitems")]
        public async Task<ActionResult> GetUserCart()
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto<object>(
                    StatusCodes.Status401Unauthorized,
                    false,
                    "User not authenticated",
                    null
                ));
            }

            var cart = await _service
                .GetUserCart(userId);

            return Ok(new ApiResponseDto<object>(
               StatusCodes.Status200OK,
               true,
               "Cart fetched successfully",
               cart
           ));
        }


        [HttpPut("updateQuantity/{cartId}")]
        public async Task<ActionResult> UpdateCartQuantity(long cartId, int quantity)
        {
            var userId = GetUserId();

            var result = await _service.UpdateCartQuantity(cartId, quantity);

            return Ok(new ApiResponseDto<object>(
                StatusCodes.Status200OK,
                true,
                "Cart quantity updated",
                result
            ));
        }


        [HttpDelete("remove/{cartId}")]
        public async Task<ActionResult> RemoveFromCart(long cartId)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto<object>(
                    StatusCodes.Status401Unauthorized,
                    false,
                    "User not authenticated",
                    null
                ));
            }
                var result = await _service
                .RemoveCart(cartId);

            return Ok(new ApiResponseDto<object>(
               StatusCodes.Status200OK,
               true,
               "Product removed from cart",
               result
           ));
        }
        [HttpDelete("clearAll")]
        public async Task<ActionResult> ClearCart()
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto<object>(
                    StatusCodes.Status401Unauthorized,
                    false,
                    "User not authenticated",
                    null
                ));
            }

            var result = await _service
                .ClearAllCart(userId);

            return Ok(new ApiResponseDto<object>(
                StatusCodes.Status200OK,
                true,
                "Cart cleared successfully",
                result
            ));
        }
    }
}
