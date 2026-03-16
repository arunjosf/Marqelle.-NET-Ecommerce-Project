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
    
    public class UserWishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public UserWishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpGet("Get")]
        public async Task<ActionResult<List<WishlistDto>>> GetUserWishlist()
        {
            var userId = GetUserIdFromToken();
            var wishlist = await _wishlistService.GetUserWishlistAsync(userId);
           
            return Ok(new ApiResponseDto<object>(
              200,
              true,
              "Wishlist fetched successfully",
              wishlist
          ));
        }
        

        [HttpPost("add")]
        public async Task<IActionResult> AddToWishlist([FromQuery] WishlistRequest request)
        {
            var userId = GetUserIdFromToken();
            await _wishlistService.AddToWishlistAsync(userId, request.ProductId);

            var wishlist = await _wishlistService.GetUserWishlistAsync(userId);


            return Ok(new ApiResponseDto<object>(
                200,
                true,
                "Product added to wishlist",
                wishlist
            ));
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> RemoveFromWishlist([FromQuery] WishlistRequest request)
        {
            var userId = GetUserIdFromToken();

            await _wishlistService.RemoveFromWishlistAsync(userId, request.ProductId);

            var wishlist = await _wishlistService.GetUserWishlistAsync(userId);

            return Ok(new ApiResponseDto<object>(
               200,
               true,
               "Product removed from wishlist",
               wishlist
           ));
        }


        private long GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (userIdClaim == null)
                throw new Exception("User ID not found in token.");

            return Convert.ToInt64(userIdClaim);
        }
        public class WishlistRequest
        {
            public long ProductId { get; set; } 
        }

        [HttpGet("debug-claims")]
        public IActionResult DebugClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(claims);
        }
    }
}

