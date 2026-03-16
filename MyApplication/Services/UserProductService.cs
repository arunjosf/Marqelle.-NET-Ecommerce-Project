using Marqelle.Application.DTO;
using Marqelle.Application.Interfaces;
using Marqelle.Domain.Entities;
using Marqelle.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.Services
{
    public class UserProductService : IUserProductService
    {
        private readonly IGenericRepository<Products> _repository;
        public UserProductService(IGenericRepository<Products> repository)
        {
            _repository = repository;
        }

        public async Task<List<ProductFetchingDto>> GetAllProducts()
        {
            var products = await _repository.GetAllAsync(
                 p => p.Category,
                 p => p.Images,
                 p => p.Stocks);
            return products.Select(MapToDto).ToList();
        }

        public async Task<List<ProductFetchingDto>> SearchProducts(string? name, string? color,string? 
            category, decimal? price, ProductSortPrice pricesort = ProductSortPrice.None,
            ProductSortRating ratingsort = ProductSortRating.None,
            ProductSortCategory? categoryFilter = ProductSortCategory.None)
            {

            var products = await _repository.GetAllAsync(
                 p => p.Category,
                 p => p.Images,
                 p => p.Stocks);

            var query = products.Where(p => !p.IsDeleted).AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(p => p.Name.ToLower().Contains(name.ToLower()));

            if (!string.IsNullOrEmpty(color))
                query = query.Where(p => p.Color.ToLower() == color.ToLower());

            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => p.Category.Name.ToLower() == category.ToLower());

            if (price.HasValue)
                query = query.Where(p => p.price <= price.Value);

            if (pricesort == ProductSortPrice.PriceAsc)
                query = query.OrderBy(p => p.price);
            else if (pricesort == ProductSortPrice.PriceDesc)
                query = query.OrderByDescending(p => p.price);

            if (ratingsort == ProductSortRating.RatingAsc)
                query = query.OrderBy(p => p.Rating);
            else if (ratingsort == ProductSortRating.RatingDesc)
                query = query.OrderByDescending(p => p.Rating);

            return query.Select(MapToDto).ToList();
        }

        public async Task<ProductFetchingDto> GetProductById(long productId)
        {
            var product = await _repository.GetByIdAsync(productId,
                 p => p.Category,
                 p => p.Images,
                 p => p.Stocks);

            if (product == null || product.IsDeleted)
                return null;

            return product == null ? null : MapToDto(product);
        }
        
        
        private static ProductFetchingDto MapToDto(Products p) => new ProductFetchingDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.price,
            Color = p.Color,

            StockInfo = (p.Stocks != null && p.Stocks.Any() && p.Stocks.All(s => s.Stock == 0))
            ? "Temporarily unavailable"
            : null,

            Rating = p.Rating,
            CategoryName = p.Category?.Name ?? string.Empty,

            Sizes = (p.Stocks != null && p.Stocks.Any())
            ? p.Stocks.Where(s => s.Stock > 0).Select(s => s.Size).ToList()
            : new List<string>(),

            Images = p.Images?.Select(i => i.ImageUrl).ToList() ?? new List<string>()   
        };
    }
}
