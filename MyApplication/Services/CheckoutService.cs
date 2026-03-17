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

            if (cartItems == null || !cartItems.Any())
                throw new Exception("Your cart is empty.");

            foreach (var item in cartItems)
            {
                if (item.IsOutOfStock)
                    throw new Exception($"'{item.ProductName}' (Size: {item.Size}) is out of stock. Please remove it to continue.");
            }

            return await BuildCheckoutDataAsync(userId, cartItems);
        }
        private async Task<CheckoutDto> BuildCheckoutDataAsync(long userId, List<UserCartDto> cartItems)
        {
            var products = cartItems.Select(c => new CheckoutProductDto
            {
                ProductId = c.ProductId,
                ProductName = c.ProductName,
                ProductImage = c.ProductImage,
                Price = c.ProductPrice,
                Quantity = c.Quantity,
                Rating = 0,
                TotalPrice = c.ProductPrice * c.Quantity
            }).ToList();

            var subtotal = products.Sum(p => p.TotalPrice);

            var userAddresses = await _addressService.GetUserAddress(userId);

            if (userAddresses == null || !userAddresses.Any())
                throw new Exception("Please add a delivery address to continue.");

            long mostRecentAddressId = userAddresses.Max(a => a.AddressId);

            var checkoutAddresses = userAddresses.Select(a => new AddressCheckoutDto
            {
                AddressId = a.AddressId,
                FullName = a.FullName,
                AddressLine = $"{a.FlatorHouseorBuildingName}, {a.LandMark}",
                CityStatePincode = $"{a.City}, {a.State} - {a.Pincode}",
                Country = a.Country,
                PhoneNumber = a.PhoneNumber,
                IsDefault = a.AddressId == mostRecentAddressId
            }).ToList();

            return new CheckoutDto
            {
                Products = products,
                Addresses = checkoutAddresses,
                SubTotal = subtotal,
                ShippingCharge = ShippingCharge,
                TotalAmount = subtotal + ShippingCharge
            };
        }
    }
}
