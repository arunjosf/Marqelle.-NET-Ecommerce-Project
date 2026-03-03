using Marqelle.Application.Interfaces;
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
            return Ok(await _upservice.GetAllProducts());
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
            return Ok(await _upservice.SearchProducts(
                name, color, category, 
                price,
                priceSort,
                ratingSort,
                categoryFilter));
        }

        [HttpGet("id")]
        public async Task<ActionResult> GetProductById(long productId)
        {
            var product = await _upservice.GetProductById(productId);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
    }
}
