using Marqelle.Application.DTO;
using Marqelle.Application.Interfaces;
using Marqelle.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marqelle.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminProductController : ControllerBase
    {
        private readonly IAdminProductService _service;

        public AdminProductController(IAdminProductService service)
        {
            _service = service;
        }

        [HttpPost("add-product")]
        public async Task<IActionResult> AddProduct([FromBody] AdminAddproductDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                return BadRequest(new ApiResponseDto<object>(
                    400,
                    false,
                    "Validation failed",
                    errors
                ));
            }
            try
            {
                Products product = await _service.AddProducts(dto);
                return Ok(new
                {
                    Success = true,
                    Message = "Product added successfully",
                    Data = new
                    {
                        product.Id,
                        product.Name,
                        product.Description,
                        product.price,
                        product.Color,
                        product.Stocks,
                        Category = dto.Category,
                        Sizes = dto.Sizes,
                        Images = dto.ImageUrls,
                        product.Rating
                    }
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponseDto<object>(
                    400,
                    false,
                    ex.Message,
                    null
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>(
                    500,
                    false,
                    "Internal server error",
                    null
                ));
            }
        }
    }
    }

