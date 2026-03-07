using Marqelle.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marqelle.Domain.Entities;

namespace Marqelle.Application.Interfaces
{
    public interface IUserCartService
    {
        Task<Cart> AddToCart(long userId, long productId, string size);
        Task<List<UserCartDto>> GetUserCart(long userId);
        Task<string> IncreaseQuantity(long cartId);
        Task<string> DecreaseQuantity(long cartId);
        Task<string> RemoveCart(long cartId);
        Task<string> ClearAllCart(long userId);
    }
}
