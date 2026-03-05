using Marqelle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.Interfaces
{
        public interface IWishlistRepository
        {
            Task AddWishlistAsync(Wishlist wishlist);
            Task RemoveWishlistAsync(Wishlist wishlist);
            Task<Wishlist?> GetWishlistAsync(long userId, long productId);
            Task<List<Wishlist>> GetUserWishlistAsync(long userId);
            Task<bool> ExistsAsync(long userId, long productId);
        }
    }

