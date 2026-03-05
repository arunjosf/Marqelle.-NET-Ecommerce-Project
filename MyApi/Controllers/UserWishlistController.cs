using Marqelle.Application.DTO;
using Marqelle.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Marqelle.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserWishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public UserWishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        // GET: api/Wishlist
        [HttpGet]
        public async Task<ActionResult<List<WishlistDto>>> GetUserWishlist()
        {
            var userId = GetUserIdFromToken();
            var wishlist = await _wishlistService.GetUserWishlistAsync(userId);
            return Ok(wishlist);
        }

        // POST: api/Wishlist
        [HttpPost]
        public async Task<IActionResult> AddToWishlist([FromBody] WishlistRequest request)
        {
            var userId = GetUserIdFromToken();
            await _wishlistService.AddToWishlistAsync(userId, request.ProductId);
            return Ok(new { message = "Product added to wishlist" });
        }

        // DELETE: api/Wishlist
        [HttpDelete]
        public async Task<IActionResult> RemoveFromWishlist([FromBody] WishlistRequest request)
        {
            var userId = GetUserIdFromToken();
            await _wishlistService.RemoveFromWishlistAsync(userId, request.ProductId);
            return Ok(new { message = "Product removed from wishlist" });
        }

        // Helper method to get userId from token

        private long GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (userIdClaim == null)
                throw new Exception("User ID not found in token.");

            return Convert.ToInt64(userIdClaim);
        }
        public class WishlistRequest
        {
            public long ProductId { get; set; } // Only need ProductId now
        }
    }
}

