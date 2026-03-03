using Marqelle.Application.Interfaces;
using Marqelle.Domain.Entities;
using Marqelle.Domain.Enums;
using Marqelle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Infrastructure.Repositories
{
    public class UserProductRepository : IUserProductRepository
    {
        private readonly ApplicationDbContext _context;

        public UserProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Products>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Sizes)
                .ToListAsync();
        }

        public async Task<List<Products>> SearchProductsAsync(string? name, string color,
            string? category, decimal? price, ProductSortPrice pricesort = ProductSortPrice.None,
            ProductSortRating ratingsort = ProductSortRating.None, ProductSortCategory? categoryFilter = ProductSortCategory.None)
        {
            var query = _context.Products
               .Include(p => p.Category)
               .Include(p => p.Images)
               .Include(p => p.Sizes)
               .AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(p => p.Name.Contains(name));

            if (price.HasValue)
                query = query.Where(p => p.price == price.Value);

            if (!string.IsNullOrEmpty(color))
                query = query.Where(p => p.Color == color);

            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => p.Category.Name == category);

            if (categoryFilter.HasValue && categoryFilter != ProductSortCategory.None)
            {
                string categoryName = categoryFilter.ToString();
                query = query.Where(p => p.Category.Name == categoryName);
            }

            query = pricesort switch
            {
                ProductSortPrice.PriceAsc => query.OrderBy(p => p.price),
                ProductSortPrice.PriceDesc => query.OrderByDescending(p => p.price),
                _ => query
            };

            query = ratingsort switch
            {
                ProductSortRating.RatingAsc => query.OrderBy(p => p.Rating),
                ProductSortRating.RatingDesc => query.OrderByDescending(p => p.Rating),
                _ => query
            };

          

            return await query.ToListAsync();
        }

    
    public async Task<Products> GetProductByIdAsync(long productId)
        {
            return await _context.Products
                                 .Include(p => p.Category)
                                 .Include(p => p.Images)
                                 .Include(p => p.Sizes)
                                 .FirstOrDefaultAsync(p => p.Id == productId);
        }
    }
}
