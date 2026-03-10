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
        private readonly IGenericRepository<Wishlist> _wishlistRepository;

        public WishlistService(IGenericRepository<Wishlist> wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public async Task AddToWishlistAsync(long userId, long productId)
        {
            var userWishlists = await _wishlistRepository.GetAllAsync(w => w.Product, w => w.Product.Images);
            
            var exists = userWishlists.FirstOrDefault(w => w.UserId == userId && w.ProductId == productId
                                                                           && w.Product != null);

            if (exists != null)
                throw new Exception("Item already in wishlist");

            var wishlist = new Wishlist
            {
                UserId = userId,
                ProductId = productId
            };

            await _wishlistRepository.AddAsync(wishlist);
            await _wishlistRepository.SaveAsync();
        }

        public async Task RemoveFromWishlistAsync(long userId, long productId)
        {
            var wishlists = await _wishlistRepository.GetAllAsync();

            var item = wishlists
                .Where(w => w != null) 
                .FirstOrDefault(w => w.UserId == userId && w.ProductId == productId);

            if (item != null)
                _wishlistRepository.Delete(item); 
            await _wishlistRepository.SaveAsync();
        }


        public async Task<List<WishlistDto>> GetUserWishlistAsync(long userId)
        {
            var wishlists = await _wishlistRepository.GetAllAsync(w => w.Product, w => w.Product.Images);

            return wishlists
                .Where(w => w.UserId == userId)
                .Select(w => new WishlistDto
                {
                    WishlistId = w.Id,
                    ProductId = w.ProductId,
                    ProductName = w.Product.Name,
                    ProductPrice = w.Product.price,
                    ProductImage = w.Product.Images.Select(i => i.ImageUrl).FirstOrDefault()
                })
                .ToList();
        }
    }
}


