using Marqelle.Application.DTO;
using Marqelle.Application.Interfaces;
using Marqelle.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marqelle.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class AdminProductController : ControllerBase
    {
        private readonly IAdminProductService _service;
        private readonly IWebHostEnvironment _env;

        public AdminProductController(IAdminProductService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        [HttpPost("upload-images")]
        public async Task<IActionResult> UploadImages([FromForm] List<IFormFile> images)
        {
            var urls = await _service.UploadImagesAsync(images, _env.WebRootPath);

            return Ok(new ApiResponseDto<List<string>>(
                StatusCodes.Status200OK, true, "Images uploaded successfully.", urls));
        }

        [HttpPost("add-product")]
        public async Task<IActionResult> AddProduct([FromBody] AdminAddproductDto dto)
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

            await _service.AddProductAsync(dto);

            return Ok(new ApiResponseDto<object>(
                StatusCodes.Status200OK, true, "Product added successfully.", null));
        }

        [HttpPut("update-product")]
        public async Task<IActionResult> UpdateProduct(
            [FromQuery] long productId,
            [FromBody] AdminUpateProductDto dto)
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

            await _service.UpdateProductAsync(productId, dto);

            return Ok(new ApiResponseDto<object>(
                StatusCodes.Status200OK, true, "Product updated successfully.", null));
        }

        [HttpGet("all-products")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _service.GetAllProductsAsync();

            return Ok(new ApiResponseDto<List<ProductFetchingDto>>(
                StatusCodes.Status200OK, true, "Products fetched successfully.", products));
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts(
            [FromQuery] long? id,
            [FromQuery] string? name,
            [FromQuery] string? color,
            [FromQuery] string? category,
            [FromQuery] decimal? price)
        {
            if (!id.HasValue && string.IsNullOrEmpty(name) && string.IsNullOrEmpty(color)
                && string.IsNullOrEmpty(category) && !price.HasValue)
            {
                return BadRequest(new ApiResponseDto<object>(
                    StatusCodes.Status400BadRequest, false,
                    "Please provide at least one search parameter.", null));
            }

            var products = await _service.SearchProductsAsync(id, name, color, category, price);

            if (!products.Any())
                return Ok(new ApiResponseDto<object>(
                    StatusCodes.Status200OK, false, "No products found.", null));

            return Ok(new ApiResponseDto<List<ProductFetchingDto>>(
                StatusCodes.Status200OK, true, $"{products.Count} product(s) found.", products));
        }


        [HttpDelete("delete-product")]
        public async Task<IActionResult> DeleteProduct([FromQuery] long productId)
        {
            await _service.DeleteProductAsync(productId, _env.WebRootPath);

            return Ok(new ApiResponseDto<object>(
                StatusCodes.Status200OK, true, "Product deleted successfully.", null));
        }

    }
}
    

