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
    public class UserCartRepository : IUserCartRepository
    {
        private readonly ApplicationDbContext _context;
        public UserCartRepository(ApplicationDbContext context)
        {
            _context = context; 
        }

        public async Task<List<Cart>> GetAllCartByUserIdAsync(long userId)
        {
            return await _context.Carts
             .Include(c => c.Product)
             .ThenInclude(p => p.Images)
             .Where(c => c.UserId == userId)
             .ToListAsync();
        }

        public async Task<Cart?> GetCartItemByProductAsync(long userId, long productId, string size)
        {
            return await _context.Carts
                .FirstOrDefaultAsync(c =>
                c.UserId == userId &&
                c.ProductId == productId &&
                c.Size == size);
        }

        public async Task AddToCartAsync(Cart cartitem)
        {
            await _context.Carts.AddAsync(cartitem);
        }

        public void UpdateCartItemAsync(Cart cartitem)
        {
            _context.Carts.Update(cartitem);
        }
        public void RemoveCartItemAsync(Cart cartitem)
        {
            _context.Carts.Remove(cartitem);
        }

        public async Task ClearUserAllCartAsync(long userId)
        {
            var items = await _context.Carts
                .Where(c => c.UserId == userId)
                .ToListAsync();
            _context.Carts.RemoveRange(items);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Cart?> GetCartItemByIdAsync(long cartId)
        {
            return await _context.Carts
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }
    }
}
