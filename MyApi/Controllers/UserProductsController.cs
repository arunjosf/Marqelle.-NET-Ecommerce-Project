using Marqelle.Application.DTO;
using Marqelle.Application.Interfaces;
using Marqelle.Domain.Entities;
using Marqelle.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marqelle.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProductsController : ControllerBase
    {
        private readonly IUserProductService _upservice;

        public UserProductsController(IUserProductService upservice)
        {
            _upservice = upservice;
        }

        [HttpGet("all")]
        public async Task<ActionResult> GetAllProducts()
        {
            var products = await _upservice.GetAllProducts();

            return Ok(new ApiResponseDto<object>(
                StatusCodes.Status200OK,
                true,
                "Products fetched successfully",
                products
            ));
        }

        [HttpGet("Search")]
        public async Task<ActionResult> SearchProducts(
            [FromQuery] string? name,
            [FromQuery] string? color,
            [FromQuery] string? category,
            [FromQuery] decimal? price = null,
            [FromQuery] ProductSortPrice priceSort = ProductSortPrice.None,
            [FromQuery] ProductSortRating ratingSort = ProductSortRating.None,
            [FromQuery] ProductSortCategory categoryFilter = ProductSortCategory.None)
        {
            var products = await _upservice.SearchProducts(
                 name, color, category,
                 price,
                 priceSort,
                 ratingSort,
                 categoryFilter);

            return Ok(new ApiResponseDto<object>(
               StatusCodes.Status200OK,
               true,
               "Search results fetched",
               products
           ));
        }

        [HttpGet("id")]
        public async Task<ActionResult> GetProductById(long productId)
        {
            

            if (productId <= 0)
            {
                return BadRequest(new ApiResponseDto<object>(
                    StatusCodes.Status400BadRequest,
                    false,
                    "Invalid product id",
                    null
                ));
            }

            var product = await _upservice.GetProductById(productId);

            if (product == null)
            {
                return NotFound(new ApiResponseDto<object>(
                    StatusCodes.Status404NotFound,
                    false,
                    "Product not found",
                    null
                ));
            }

                return Ok(new ApiResponseDto<object>(
                StatusCodes.Status200OK,
                true,
                "Product fetched successfully",
                product
    ));
        }
        }
    }

