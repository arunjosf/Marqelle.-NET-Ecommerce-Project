using Marqelle.Application.DTO;
using Marqelle.Application.Interfaces;
using Marqelle.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marqelle.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddAddress(
            [FromQuery] string addressType,
            [FromQuery] string fullName,
            [FromQuery] string phoneNumber,
            [FromQuery] string email,
            [FromQuery] string country,
            [FromQuery] string state,
            [FromQuery] string city,
            [FromQuery] string pincode,
            [FromQuery] string flatorHouseorBuildingName,
            [FromQuery] string landMark)
        {
            var userId = GetUserIdFromCookie();

            var dto = new AddressDto
            {
                AddressType = addressType,
                FullName = fullName,
                PhoneNumber = phoneNumber,
                Email = email,
                Country = country,
                State = state,
                City = city,
                Pincode = pincode,
                FlatorHouseorBuildingName = flatorHouseorBuildingName,
                LandMark = landMark
            };

            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto<object>(
                    StatusCodes.Status401Unauthorized,
                    false,
                    "User not authenticated",
                    null
                ));
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
               .SelectMany(v => v.Errors)
               .Select(e => e.ErrorMessage)
               .ToList();

                return BadRequest(new ApiResponseDto<List<string>>(
                        StatusCodes.Status400BadRequest, false, "Validation Failed", errors));
            }

                await _addressService.AddAddress(userId, dto);

                return Ok(new ApiResponseDto<object>(StatusCodes.Status200OK, true, "Address added successfully", null));
            
        }

        [HttpGet("user-addresses")]
        public async Task<IActionResult> GetUserAddresses()
        {
            var userId = GetUserIdFromCookie();

            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto<object>(
                    StatusCodes.Status401Unauthorized,
                    false,
                    "User not authenticated",
                    null
                ));
            }

            var addresses = await _addressService.GetUserAddress(userId);

            return Ok(new ApiResponseDto<List<AddressDto>>(
                StatusCodes.Status200OK,
                true,
                "User addresses fetched successfully",
                addresses
            ));
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateAddress(
            [FromQuery] long addressId,
            [FromQuery] string addressType,
            [FromQuery] string fullName,
            [FromQuery] string phoneNumber,
            [FromQuery] string email,
            [FromQuery] string country,
            [FromQuery] string state,
            [FromQuery] string city,
            [FromQuery] string pincode,
            [FromQuery] string flatorHouseorBuildingName,
            [FromQuery] string landMark)
        {
            var dto = new AddressDto
            {
                AddressType = addressType,
                FullName = fullName,
                PhoneNumber = phoneNumber,
                Email = email,
                Country = country,
                State = state,
                City = city,
                Pincode = pincode,
                FlatorHouseorBuildingName = flatorHouseorBuildingName,
                LandMark = landMark
            };

            var userId = GetUserIdFromCookie();

            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto<object>(
                    StatusCodes.Status401Unauthorized,
                    false,
                    "User not authenticated",
                    null
                ));
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
               .SelectMany(v => v.Errors)
               .Select(e => e.ErrorMessage)
               .ToList();

                return BadRequest(new ApiResponseDto<List<string>>(
                    StatusCodes.Status400BadRequest,
                    false,
                    "Validation failed",
                    errors
                ));
            }
                await _addressService.UpdateAddressAsync(addressId, userId, dto);

            return Ok(new ApiResponseDto<object>(
                StatusCodes.Status200OK,
                true,
                "Address updated successfully",
                null
            ));
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAddress([FromQuery] long addressId)
        {
            var userId = GetUserIdFromCookie();

            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto<object>(
                    StatusCodes.Status401Unauthorized,
                    false,
                    "User not authenticated",
                    null
                ));
            }
            await _addressService.DeleteAddress(addressId);

            return Ok(new ApiResponseDto<object>(
               StatusCodes.Status200OK,
               true,
               "Address deleted successfully",
               null
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
