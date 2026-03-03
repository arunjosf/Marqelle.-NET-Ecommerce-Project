using Marqelle.Application.Interfaces;
using Marqelle.Domain.Entities;
using Marqelle.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Infrastructure.Repositories
{
    public class AdminProductRepository : IAdminProductRepository
    {

        private readonly ApplicationDbContext _context;

        public AdminProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Products> AddProductsAsync(Products products, string categoryName)
        {
            var category = await _context.Categories
               .FirstOrDefaultAsync(c => c.Name.ToLower() == categoryName.ToLower());

            if (category == null)
            {
                category = new ProductsCategory { Name = categoryName };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }

            products.CategoryId = category.Id;

            _context.Products.Add(products);
            await _context.SaveChangesAsync();

            return products;
        }
    }
}