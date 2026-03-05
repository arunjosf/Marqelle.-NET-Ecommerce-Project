using Marqelle.Application.DTO;
using Marqelle.Application.Interfaces;
using Marqelle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;



namespace Marqelle.Application.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;

        public WishlistService(IWishlistRepository _wishlistRepository)
        {
            this._wishlistRepository = _wishlistRepository;
        }

        public async Task AddToWishlistAsync(long userId, long productId)
        {
            var exists = await _wishlistRepository.ExistsAsync(userId, productId);
            if (exists) return;

            var wishlist = new Wishlist
            {
                UserId = userId,
                ProductId = productId
            };

            await _wishlistRepository.AddWishlistAsync(wishlist);
        }

        public async Task RemoveFromWishlistAsync(long userId, long productId)
        {
            var item = await _wishlistRepository.GetWishlistAsync(userId, productId);
            if (item != null)
                await _wishlistRepository.RemoveWishlistAsync(item);
        }

        public async Task<List<WishlistDto>> GetUserWishlistAsync(long userId)
        {
            var wishlists = await _wishlistRepository.GetUserWishlistAsync(userId);

            
            return wishlists
           .Where(w => w.Product != null)
           .Select(w => new WishlistDto
           {
               WishlistId = w.Id,
               ProductId = w.ProductId,
               ProductName = w.Product?.Name ?? "Unknown",
               ProductImage = w.Product.Images?.Select(i => i.ImageUrl).FirstOrDefault() ?? "",
               ProductPrice = w.Product?.price ?? 0
           })
           .ToList();
        }
    }
}


