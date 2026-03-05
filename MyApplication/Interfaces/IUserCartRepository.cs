using Marqelle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.Interfaces
{
    public interface IUserCartRepository
    {
        Task<List<Cart>> GetAllCartByUserIdAsync(long userId);
        Task<Cart?> GetCartItemByProductAsync(long userId, long productId, string size);
        Task AddToCartAsync(Cart cartitem);
        void UpdateCartItemAsync(Cart cartitem);
        void RemoveCartItemAsync(Cart cartitem);
        Task ClearUserAllCartAsync(long userId);
        Task SaveChangesAsync();
        Task<Cart?> GetCartItemByIdAsync(long cartId);

    }
}
