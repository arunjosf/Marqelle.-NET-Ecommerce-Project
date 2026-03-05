using Marqelle.Domain.Entities;
using Marqelle.Infrastructure.Data;
using Marqelle.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Marqelle.Infrastructure.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly ApplicationDbContext _context;

        public WishlistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddWishlistAsync(Wishlist wishlist)
        {
            await _context.Wishlists.AddAsync(wishlist);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveWishlistAsync(Wishlist wishlist)
        {
            _context.Wishlists.Remove(wishlist);
            await _context.SaveChangesAsync();
        }

        public async Task<Wishlist?> GetWishlistAsync(long userId, long productId)
        {
            return await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
        }

        public async Task<List<Wishlist>> GetUserWishlistAsync(long userId)
        {
            return await _context.Wishlists
        .Include(w => w.Product)                 
            .ThenInclude(p => p.Images)         
        .Where(w => w.UserId == userId)
        .ToListAsync();
        }

        public async Task<bool> ExistsAsync(long userId, long productId)
        {
            return await _context.Wishlists
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId);
        }
    }
}