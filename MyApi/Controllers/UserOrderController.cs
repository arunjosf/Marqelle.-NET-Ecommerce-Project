using Marqelle.Application.DTO;
using Marqelle.Application.Interfaces;
using Marqelle.Domain.Entities;
using Marqelle.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marqelle.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserOrderController : ControllerBase
    {
        private readonly IUserOrderService _orderService;

        public UserOrderController(IUserOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("place-order")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponseDto<List<string>>(
                    StatusCodes.Status400BadRequest, false, "Validation failed.", errors));
            }

            var addressIdStr = Request.Query["addressId"].ToString();
            if (string.IsNullOrEmpty(addressIdStr) || !long.TryParse(addressIdStr, out long addressId))
            {
                return BadRequest(new ApiResponseDto<string>(
                    StatusCodes.Status400BadRequest, false, "Invalid or missing addressId parameter.", null));
            }

            dto.AddressId = addressId;
            var userId = GetUserId();
            var result = await _orderService.PlaceOrderAsync(userId, dto);

            return Ok(new ApiResponseDto<UserOrderResponseDto>(
                StatusCodes.Status200OK, true, "Order placed successfully.", result));
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetUserId();
            var orders = await _orderService.GetUserOrdersAsync(userId);

            return Ok(new ApiResponseDto<List<UserOrderHistoryDto>>(
                StatusCodes.Status200OK, true, "Orders fetched successfully.", orders));
        }

        [HttpPut("cancel-order")]
        public async Task<IActionResult> CancelOrder([FromQuery] long orderId)
        {
            var userId = GetUserId();
            await _orderService.CancelOrderAsync(orderId, userId);

            return Ok(new ApiResponseDto<object>(
                StatusCodes.Status200OK, true, "Order cancelled successfully.", null));
        }

        private long GetUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (userIdClaim == null)
                throw new Exception("User ID not found in token.");

            return Convert.ToInt64(userIdClaim);
        }

        [HttpPost("create-razorpay-order")]
        public async Task<IActionResult> CreateRazorpayOrder([FromQuery] long addressId)
        {
            var userId = GetUserId();
            var result = await _orderService.CreateRazorpayOrderAsync(userId, addressId);
            return Ok(new ApiResponseDto<RazorPayOrderDto>(200, true, "Razorpay order created.", result));
        }

        [HttpPost("verify-and-place-order")]
        public async Task<IActionResult> VerifyAndPlaceOrder([FromBody] VerifyPaymentDto dto)
        {
            var userId = GetUserId();
            var result = await _orderService.VerifyAndPlaceOrderAsync(userId, dto);
            return Ok(new ApiResponseDto<UserOrderResponseDto>(200, true, "Order placed successfully.", result));
        }
    }
}

