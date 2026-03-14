using Marqelle.Application.DTO;
using Marqelle.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marqelle.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminUserController : ControllerBase
    {
        private readonly IAdminUserService _adminUserService;

        public AdminUserController(IAdminUserService adminUserService)
        {
            _adminUserService = adminUserService;
        }

        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminUserService.GetAllUsersAsync();

            return Ok(new ApiResponseDto<List<AdminUserManagementDto>>(
                StatusCodes.Status200OK, true, "Users fetched successfully.", users));
        }


        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers(
            [FromQuery] long? id,
            [FromQuery] string? name)
        {
            if (!id.HasValue && string.IsNullOrEmpty(name))
                return BadRequest(new ApiResponseDto<object>(
                    StatusCodes.Status400BadRequest, false,
                    "Please provide at least one search parameter: id or name.", null));

            var users = await _adminUserService.SearchUsersAsync(id, name);

            if (!users.Any())
                return Ok(new ApiResponseDto<object>(
                    StatusCodes.Status200OK, false, "No users found.", null));

            return Ok(new ApiResponseDto<List<AdminUserManagementDto>>(
                StatusCodes.Status200OK, true, $"{users.Count} user(s) found.", users));
        }

        [HttpPut("block")]
        public async Task<IActionResult> BlockUser([FromQuery] long userId)
        {
            await _adminUserService.BlockUserAsync(userId);

            return Ok(new ApiResponseDto<object>(
                StatusCodes.Status200OK, true, "User blocked successfully.", null));
        }

        [HttpPut("unblock")]
        public async Task<IActionResult> UnblockUser([FromQuery] long userId)
        {
            await _adminUserService.UnblockUserAsync(userId);

            return Ok(new ApiResponseDto<object>(
                StatusCodes.Status200OK, true, "User unblocked successfully.", null));
        }
    }
}
