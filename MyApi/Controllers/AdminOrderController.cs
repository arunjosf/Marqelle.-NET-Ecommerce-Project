using Marqelle.Application.DTO;
using Marqelle.Application.Interfaces;
using Marqelle.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marqelle.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class AdminOrderController : ControllerBase
    {
        private readonly IAdminOrderService _adminOrderService;

        public AdminOrderController(IAdminOrderService adminOrderService)
        {
            _adminOrderService = adminOrderService;
        }

        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateOrderStatus(
            [FromQuery] long orderId,
            [FromQuery] OrderStatus status)
        {
            await _adminOrderService.UpdateOrderStatusAsync(orderId, status);

            return Ok(new ApiResponseDto<object>(
                StatusCodes.Status200OK, true, $"Order status updated to {status}.", null));
        }

        [HttpGet("all-orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _adminOrderService.GetAllOrdersAsync();

            return Ok(new ApiResponseDto<List<AdminOrderHistoryDto>>(
                StatusCodes.Status200OK, true, "All orders fetched successfully.", orders));
        }


        [HttpGet("search")]
        public async Task<IActionResult> SearchOrders([FromForm] long? orderId,[FromForm] long? productId,[FromForm] long? userId)
        {
            if (!orderId.HasValue && !productId.HasValue && !userId.HasValue)
                return BadRequest(new ApiResponseDto<object>(
                    StatusCodes.Status400BadRequest, false,
                    "Please provide at least one search parameter: orderId, productId, or userId.", null));

            var orders = await _adminOrderService.SearchOrdersAsync(orderId, productId, userId);

            if (!orders.Any())
                return Ok(new ApiResponseDto<object>(
                    StatusCodes.Status200OK, false, "No orders found matching the search criteria.", null));

            return Ok(new ApiResponseDto<List<AdminOrderHistoryDto>>(
                StatusCodes.Status200OK, true, $"{orders.Count} order(s) found.", orders));
        }
     
    }
}
