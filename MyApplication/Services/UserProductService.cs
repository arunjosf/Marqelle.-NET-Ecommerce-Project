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
        private readonly IUserProductRepository _repository;
        public UserProductService(IUserProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProductFetchingDto>> GetAllProducts()
        {
            var products = await _repository.GetAllProductsAsync();
            return products.Select(MapToDto).ToList();
        }

        public async Task<List<ProductFetchingDto>> SearchProducts(string? name, string color,
            string? category, decimal? price, ProductSortPrice pricesort = ProductSortPrice.None,
            ProductSortRating ratingsort = ProductSortRating.None, ProductSortCategory? categoryFilter = ProductSortCategory.None)
        {
            var products =  await _repository.SearchProductsAsync(name, color, category, price, pricesort, ratingsort, categoryFilter);
            return products.Select(MapToDto).ToList();
        }

        public async Task<ProductFetchingDto> GetProductById(long productId)
        {
            var product = await _repository.GetProductByIdAsync(productId);
            return product == null ? null : MapToDto(product);
        }

        private static ProductFetchingDto MapToDto(Products p) => new ProductFetchingDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.price,
            Color = p.Color,
            InStock = p.InStock,
            Rating = p.Rating,
            CategoryName = p.Category?.Name ?? string.Empty,
            Sizes = p.Sizes?.Select(s => s.Size).ToList() ?? new List<string>(),
            Images = p.Images?.Select(i => i.ImageUrl).ToList() ?? new List<string>()
        };
    }
}
