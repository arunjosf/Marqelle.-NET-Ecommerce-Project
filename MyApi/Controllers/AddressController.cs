using Marqelle.Application.DTO;
using Marqelle.Application.Interfaces;
using Marqelle.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marqelle.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        // ADD ADDRESS
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

            await _addressService.AddAddress(userId, dto);

            return Ok("Address added successfully");
        }

        // GET USER ADDRESSES
        [HttpGet("user-addresses")]
        public async Task<IActionResult> GetUserAddresses()
        {
            var userId = GetUserIdFromCookie();

            var addresses = await _addressService.GetUserAddress(userId);

            return Ok(addresses);
        }

        // UPDATE ADDRESS
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

            await _addressService.UpdateAddressAsync(addressId, dto);

            return Ok("Address updated successfully");
        }

        // DELETE ADDRESS
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAddress([FromQuery] long addressId)
        {
            await _addressService.DeleteAddress(addressId);

            return Ok("Address deleted successfully");
        }

        // CHECKOUT ADDRESS
        [HttpGet("checkout-address")]
        public async Task<IActionResult> GetCheckoutAddress()
        {
            var userId = GetUserIdFromCookie();

            var address = await _addressService.GetCheckoutAddressAsync(userId);

            return Ok(address);
        }

        // GET ALL CHECKOUT ADDRESSES
        [HttpGet("checkout-addresses")]
        public async Task<IActionResult> GetCheckoutAddresses()
        {
            var userId = GetUserIdFromCookie();

            var addresses = await _addressService.GetCheckoutAddressesAsync(userId);

            return Ok(addresses);
        }

        // READ USERID FROM COOKIE
        private long GetUserIdFromCookie()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (userIdClaim == null)
                throw new Exception("User ID not found in token.");

            return Convert.ToInt64(userIdClaim);
        }
    }
}
