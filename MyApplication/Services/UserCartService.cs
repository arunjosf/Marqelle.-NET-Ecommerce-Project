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
        private readonly IGenericRepository<Cart> _cartRepository;
        private readonly IGenericRepository<ProductSizeAndStock> _stockRepository;
        public UserCartService(IGenericRepository<Cart> cartRespository, IGenericRepository<ProductSizeAndStock> stockRepository)
        {
            _cartRepository = cartRespository;
            _stockRepository = stockRepository;
        }

        public async Task<Cart> AddToCart(long userId, long productId, string size)
        {
            if (string.IsNullOrWhiteSpace(size))
                throw new Exception("Please select a size");

            size = size.Trim().ToUpper();

            var stock = await _stockRepository.FindAsync(s =>
            s.ProductId == productId &&
            s.Size.ToUpper() == size);

            if (stock == null)
                throw new Exception("Selected size is not available for this product");

            if (stock.Stock <= 0)
                throw new Exception("Selected size is out of stock");

            var existingCart = await _cartRepository.FindAsync(c =>
                c.UserId == userId &&
                c.ProductId == productId &&
                c.Size == size);

            if (existingCart != null)
                throw new Exception("This product with the selected size is already in your cart");

            var cartItem = new Cart
            {
                UserId = userId,
                ProductId = productId,
                Size = size,
                Quantity = 1
            };

            await _cartRepository.AddAsync(cartItem);
            await _cartRepository.SaveAsync();

            return cartItem;
        }

        public async Task<List<UserCartDto>> GetUserCart(long userId)
        {
            var cartItems = await _cartRepository
                .GetAllAsync(c => c.Product, c => c.Product.Images);

            var userCart = cartItems
                .Where(c => c.UserId == userId)
                .ToList();

            var cartDto = new List<UserCartDto>();

            foreach (var c in userCart)
            {
                var stock = await _stockRepository.FindAsync(s =>
                    s.ProductId == c.ProductId &&
                    s.Size == c.Size);

                bool isOutOfStock = stock == null || stock.Stock == 0;
                string? warning = null;

                if (stock != null && stock.Stock < 5 && stock.Stock > 0)
                {
                    warning = $"Only {stock.Stock} items left";
                }

                cartDto.Add(new UserCartDto
                {
                    CartId = c.Id,
                    ProductId = c.ProductId,
                    ProductName = c.Product.Name,
                    ProductImage = c.Product.Images.Select(i => i.ImageUrl).FirstOrDefault(),
                    ProductPrice = c.Product.price,
                    Quantity = c.Quantity,
                    Size = c.Size,
                    StockWarning = warning,
                    IsOutOfStock = isOutOfStock,
                });
            }

            var totalPrice = cartDto.Sum(x => x.TotalPrice);
            var totalQuantity = cartDto.Sum(x => x.Quantity);

            foreach (var item in cartDto)
            {
                item.TotalCartPrice = totalPrice;
                item.TotalCartQuantity = totalQuantity;
            }

            return cartDto;
        }

        public async Task<Cart> UpdateCartQuantity(long cartId, int quantity)
        {
            if (quantity < 1)
                throw new Exception("Quantity cannot be less than 1");

            var cartItem = await _cartRepository.GetByIdAsync(cartId);

            if (cartItem == null)
                throw new Exception("Cart item not found");

            var stock = await _stockRepository.FindAsync(s =>
                s.ProductId == cartItem.ProductId &&
                s.Size == cartItem.Size);

            if (stock == null)
                throw new Exception("Stock not found");

            if (quantity > stock.Stock)
                throw new Exception($"Only {stock.Stock} items available");

            cartItem.Quantity = quantity;

            _cartRepository.Update(cartItem);
            await _cartRepository.SaveAsync();
            return cartItem;
        }

        public async Task<string> RemoveCart(long cartId)
        {
            var item = await _cartRepository.GetByIdAsync(cartId);

            if (item == null)
                return "Cart item not found.";

            _cartRepository.Delete(item);
            await _cartRepository.SaveAsync();

            return "Item removed from cart.";
        }

        public async Task<string> ClearAllCart(long userId)
        {
            var carts = await _cartRepository.GetAllAsync();

            var userCarts = carts.Where(c => c.UserId == userId).ToList();

            foreach (var cart in userCarts)
            {
                _cartRepository.Delete(cart);
            }

            await _cartRepository.SaveAsync();

            return "Cart cleared successfully.";
        }
    }
}
