using Marqelle.Application.DTO;
using Marqelle.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IUserCartService _cartService;
        private readonly IAddressService _addressService;
        private const decimal ShippingCharge = 40;

        public CheckoutService(IUserCartService cartService, IAddressService addressService)
        {
            _cartService = cartService;
            _addressService = addressService;
        }

        public async Task<CheckoutDto> GetCheckoutPageAsync(long userId)
        {
            var cartItems = await _cartService.GetUserCart(userId); 
            var products = cartItems.Select(c => new CheckoutProductDto
            {
                ProductId = c.ProductId,
                ProductName = c.ProductName,
                ProductImage = c.ProductImage,
                Price = c.ProductPrice,
                Quantity = c.Quantity,
                Rating = 0, 
                TotalPrice = c.TotalPrice
            }).ToList();

           
            var subtotal = products.Sum(p => p.TotalPrice);

            var addresses = await _addressService.GetCheckoutAddressesAsync(userId);

            return new CheckoutDto
            {
                Products = products,
                Addresses = addresses,
                SubTotal = subtotal,
                ShippingCharge = ShippingCharge,
                TotalAmount = subtotal + ShippingCharge
            };
        }
    }
}
