using Marqelle.Application.DTO;
using Marqelle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.Interfaces
{ 
        public interface IWishlistService
        {
            Task AddToWishlistAsync(long userId, long productId);
            Task RemoveFromWishlistAsync(long userId, long productId);
            Task<List<WishlistDto>> GetUserWishlistAsync(long userId);
        }
    }

