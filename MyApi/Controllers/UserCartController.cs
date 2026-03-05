using Marqelle.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Marqelle.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<IActionResult> AddToCart(long productId, string size)
        {
            var userId = GetUserId();

            var result = await _service
                .AddToCart(userId, productId, size);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserCart()
        {
            var userId = GetUserId();

            var cart = await _service
                .GetUserCart(userId);

            return Ok(cart);
        }

        [HttpPut("increase/{cartId}")]
        public async Task<IActionResult> IncreaseQuantity(long cartId)
        {
            var result = await _service
                .IncreaseQuantity(cartId);

            return Ok(result);
        }

        [HttpPut("decrease/{cartId}")]
        public async Task<IActionResult> DecreaseQuantity(long cartId)
        {
            var result = await _service
                .DecreaseQuantity(cartId);

            return Ok(result);
        }

        [HttpDelete("remove/{cartId}")]
        public async Task<IActionResult> RemoveFromCart(long cartId)
        {
            var result = await _service
                .RemoveCart(cartId);

            return Ok(result);
        }
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();

            var result = await _service
                .ClearAllCart(userId);

            return Ok(result);
        }
    }
}
