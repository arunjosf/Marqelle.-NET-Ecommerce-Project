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


        /// <summary>
        /// Returns the checkout page data including cart products, addresses, subtotal, shipping, and total amount.
        /// Marks the most recent address as the default.
        /// </summary>
        public async Task<CheckoutDto> GetCheckoutPageAsync(long userId)
        {
            // 1. Get and validate cart
            var cartItems = await _cartService.GetUserCart(userId);

            if (cartItems == null || !cartItems.Any())
                throw new Exception("Your cart is empty.");

            foreach (var item in cartItems)
            {
                if (item.IsOutOfStock)
                    throw new Exception($"'{item.ProductName}' (Size: {item.Size}) is out of stock. Please remove it to continue.");
            }

            // 2. Map cart items
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

            // 3. Get user addresses — already ordered most-recent-first (by Id desc) from AddressService
            var userAddresses = await _addressService.GetUserAddress(userId);

            if (userAddresses == null || !userAddresses.Any())
                throw new Exception("Please add a delivery address to continue.");

            // 4. The address with the highest AddressId is the most recent — mark it as default.
            //    We find the max AddressId here and use it for comparison so IsDefault is set
            //    correctly regardless of any ordering or mapping issues upstream.
            long mostRecentAddressId = userAddresses.Max(a => a.AddressId);

            var checkoutAddresses = userAddresses.Select(a => new AddressCheckoutDto
            {
                AddressId = a.AddressId,
                FullName = a.FullName,
                AddressLine = $"{a.FlatorHouseorBuildingName}, {a.LandMark}",
                CityStatePincode = $"{a.City}, {a.State} - {a.Pincode}",
                Country = a.Country,
                PhoneNumber = a.PhoneNumber,
                IsDefault = a.AddressId == mostRecentAddressId   // ← clean, no DB column needed
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
