using Marqelle.Application.DTO;
using Marqelle.Application.Interfaces;
using Marqelle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.Services
{
    public class UserCartService : IUserCartService
    {
        private readonly IUserCartRepository _repository;
        public UserCartService(IUserCartRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> AddToCart(long userId, long productId, string size)
        {
            var existItem = await _repository
                .GetCartItemByProductAsync(userId, productId, size);

            if (existItem != null)
            {
                return "Item already exists in cart.";
            }

            var cartItem = new Cart
            {
                UserId = userId,
                ProductId = productId,
                Quantity = 1,
                Size = size
            };

            await _repository.AddToCartAsync(cartItem);
            await _repository.SaveChangesAsync();
            return "Item added to cart successfully.";
        }
        public async Task<List<UserCartDto>> GetUserCart(long userId)
        {
            var cartItems = await _repository
                .GetAllCartByUserIdAsync(userId);

            var cartDto = cartItems.Select(c => new UserCartDto
            {
                CartId = c.Id,
                ProductId = c.ProductId,
                ProductName = c.Product.Name,
                ProductImage = c.Product.Images.Select(i => i.ImageUrl).FirstOrDefault(),
                ProductPrice = c.Product.price,
                Quantity = c.Quantity,
                Size = c.Size
            }).ToList();

            var totalPrice = cartDto.Sum(x => x.TotalPrice);
            var totalQuantity = cartDto.Sum(x => x.TotalCartQuantity);

            foreach (var item in cartDto)
            {
                item.TotalCartPrice = totalPrice;
                item.TotalCartQuantity = totalQuantity;
            }
            return cartDto;
        }

        public async Task<string> IncreaseQuantity(long cartId)
        {
            var item = await _repository.GetCartItemByIdAsync(cartId);

            if (item == null)
                return "Cart item not found.";

            item.Quantity += 1;

            _repository.UpdateCartItemAsync(item);
            await _repository.SaveChangesAsync();

            return "Quantity increased.";
        }
        public async Task<string> DecreaseQuantity(long cartId)
        {
            var item = await _repository.GetCartItemByIdAsync(cartId);

            if (item == null)
                return "Cart item not found.";

            if (item.Quantity <= 1)
                return "Quantity cannot be less than 1.";

            item.Quantity -= 1;

            _repository.UpdateCartItemAsync(item);
            await _repository.SaveChangesAsync();

            return "Quantity decreased.";

        }

        public async Task<string> RemoveCart(long cartId)
        {
            var item = await _repository.GetCartItemByIdAsync(cartId);

            if (item == null)
                return "Cart item not found.";

            _repository.RemoveCartItemAsync(item);
            await _repository.SaveChangesAsync();

            return "Item removed from cart.";
        }

        public async Task<string> ClearAllCart(long userId)
        {
            await _repository.ClearUserAllCartAsync(userId);
            await _repository.SaveChangesAsync();

            return "Cart cleared successfully.";
        }
    }
}
