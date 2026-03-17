//using Marqelle.Application.Interfaces;
//using Marqelle.Domain.Entities;
//using Marqelle.Domain.Enums;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Marqelle.Application.DTO;
//namespace Marqelle.Application.Services
//{
//    public class UserOrderService : IUserOrderService
//    {
//        private readonly IUserCartService _cartService;
//        private readonly IAddressService _addressService;
//        private readonly IGenericRepository<Orders> _orderRepository;
//        private readonly IGenericRepository<OrderItems> _orderItemsRepository;
//        private readonly IGenericRepository<Payments> _paymentRepository;
//        private readonly IGenericRepository<ProductSizeAndStock> _stockRepository;
//        private const decimal ShippingCharge = 40;

//        public UserOrderService(
//            IUserCartService cartService,
//            IAddressService addressService,
//            IGenericRepository<Orders> orderRepository,
//            IGenericRepository<OrderItems> orderItemsRepository,
//            IGenericRepository<Payments> paymentRepository,
//            IGenericRepository<ProductSizeAndStock> stockRepository)
//        {
//            _cartService = cartService;
//            _addressService = addressService;
//            _orderRepository = orderRepository;
//            _orderItemsRepository = orderItemsRepository;
//            _paymentRepository = paymentRepository;
//            _stockRepository = stockRepository;
//        }

//        public async Task<UserOrderResponseDto> PlaceOrderAsync(long userId, PlaceOrderDto dto)
//        {
//            var userAddresses = await _addressService.GetUserAddress(userId);
//            var selectedAddress = userAddresses.FirstOrDefault(a => a.AddressId == dto.AddressId);

//            if (selectedAddress == null)
//                throw new Exception("Selected address not found. Please choose a valid delivery address.");

//            var cartItems = await _cartService.GetUserCart(userId);

//            if (cartItems == null || !cartItems.Any())
//                throw new Exception("Your cart is empty.");

//            foreach (var item in cartItems)
//            {
//                if (item.IsOutOfStock)
//                    throw new Exception($"'{item.ProductName}' (Size: {item.Size}) is out of stock. Please remove it to continue.");
//            }

//            var subtotal = cartItems.Sum(c => c.ProductPrice * c.Quantity);
//            var totalAmount = subtotal + ShippingCharge;

//            var order = new Orders
//            {
//                UserId = userId,
//                OrderDateTime = DateTime.UtcNow,
//                TotalAmount = totalAmount,
//                Status = OrderStatus.Pending
//            };

//            await _orderRepository.AddAsync(order);
//            await _orderRepository.SaveAsync();

//            foreach (var item in cartItems)
//            {
//                var orderItem = new OrderItems
//                {
//                    OrderId = order.Id,
//                    ProductId = item.ProductId,
//                    Quantity = item.Quantity,
//                    Price = item.ProductPrice,
//                    ProductName = item.ProductName,   
//                    ProductImage = item.ProductImage,
//                    Size = item.Size
//                };

//                await _orderItemsRepository.AddAsync(orderItem);

//                var stock = await _stockRepository.FindAsync(s =>
//                    s.ProductId == item.ProductId &&
//                    s.Size == item.Size);

//                if (stock != null)
//                {
//                    stock.Stock -= item.Quantity;
//                    _stockRepository.Update(stock);
//                }
//            }

//            await _orderItemsRepository.SaveAsync();

//            var paymentStatus = dto.PaymentMethod == PaymentMethods.COD
//                ? PaymentStatus.Pending
//                : PaymentStatus.Success;

//            var payment = new Payments
//            {
//                OrderId = order.Id,
//                Amount = totalAmount,
//                PaymentMethod = dto.PaymentMethod.ToString(),
//                Status = paymentStatus,
//                PaidAt = DateTime.UtcNow,
//                TransactionId = Guid.NewGuid().ToString()
//            };

//            await _paymentRepository.AddAsync(payment);
//            await _paymentRepository.SaveAsync();

//            await _cartService.ClearAllCart(userId);

//            return new UserOrderResponseDto
//            {
//                OrderId = order.Id,
//                OrderDateTime = order.OrderDateTime,
//                TotalAmount = order.TotalAmount,
//                PaymentMethod = dto.PaymentMethod.ToString(),
//                PaymentStatus = paymentStatus.ToString(),
//                OrderStatus = order.Status.ToString()
//            };
//        }

//        public async Task CancelOrderAsync(long orderId, long userId)
//        {
//            var order = await _orderRepository.GetByIdAsync(orderId);

//            if (order == null || order.UserId != userId)
//                throw new Exception("Order not found or you are not authorized.");

//            if (order.Status == OrderStatus.Delivered)
//                throw new Exception("Delivered orders cannot be cancelled.");

//            if (order.Status == OrderStatus.Cancelled)
//                throw new Exception("Order is already cancelled.");

//            var orderItems = await _orderItemsRepository.FindAllAsync(i => i.OrderId == orderId);

//            foreach (var item in orderItems)
//            {
//                var stock = await _stockRepository.FindAsync(s =>
//                    s.ProductId == item.ProductId && s.Size == item.Size);

//                if (stock != null)
//                {
//                    stock.Stock += item.Quantity;
//                    _stockRepository.Update(stock);
//                }
//            }

//            order.Status = OrderStatus.Cancelled;
//            _orderRepository.Update(order);
//            await _orderRepository.SaveAsync();
//        }

//        public async Task<List<UserOrderHistoryDto>> GetUserOrdersAsync(long userId)
//        {

//            var orders = await _orderRepository.FindAllAsync(o => o.UserId == userId);
//            var orderIds = orders.Select(o => o.Id).ToList();
//            var allItems = await _orderItemsRepository.FindAllAsync(i => orderIds.Contains(i.OrderId));

//            return orders
//                .OrderByDescending(o => o.OrderDateTime)
//                .Select(o => new UserOrderHistoryDto
//                {
//                    OrderId = o.Id,
//                    OrderedDate = o.OrderDateTime,
//                    Status = o.Status.ToString(),
//                    TotalAmount = o.TotalAmount,
//                    Products = allItems
//                        .Where(i => i.OrderId == o.Id)
//                        .Select(i => new OrderItemDto
//                        {
//                            ProductId = i.ProductId,
//                            ProductName = i.ProductName,
//                            ProductImage = i.ProductImage,
//                            Size = i.Size,
//                            Quantity = i.Quantity,
//                            Price = i.Price,
//                            TotalPrice = i.Price * i.Quantity
//                        }).ToList()
//                }).ToList();
//        }
//    }
//}

using Marqelle.Application.Interfaces;
using Marqelle.Domain.Entities;
using Marqelle.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marqelle.Application.DTO;
namespace Marqelle.Application.Services
{
    public class UserOrderService : IUserOrderService
    {
        private readonly IUserCartService _cartService;
        private readonly IAddressService _addressService;
        private readonly IGenericRepository<Orders> _orderRepository;
        private readonly IGenericRepository<OrderItems> _orderItemsRepository;
        private readonly IGenericRepository<Payments> _paymentRepository;
        private readonly IGenericRepository<ProductSizeAndStock> _stockRepository;
        private const decimal ShippingCharge = 40;

        public UserOrderService(
            IUserCartService cartService,
            IAddressService addressService,
            IGenericRepository<Orders> orderRepository,
            IGenericRepository<OrderItems> orderItemsRepository,
            IGenericRepository<Payments> paymentRepository,
            IGenericRepository<ProductSizeAndStock> stockRepository)
        {
            _cartService = cartService;
            _addressService = addressService;
            _orderRepository = orderRepository;
            _orderItemsRepository = orderItemsRepository;
            _paymentRepository = paymentRepository;
            _stockRepository = stockRepository;
        }

        public async Task<UserOrderResponseDto> PlaceOrderAsync(long userId, PlaceOrderDto dto)
        {
            var userAddresses = await _addressService.GetUserAddress(userId);
            var selectedAddress = userAddresses.FirstOrDefault(a => a.AddressId == dto.AddressId);

            if (selectedAddress == null)
                throw new Exception("Selected address not found. Please choose a valid delivery address.");

            var cartItems = await _cartService.GetUserCart(userId);

            if (cartItems == null || !cartItems.Any())
                throw new Exception("Your cart is empty.");

            foreach (var item in cartItems)
            {
                if (item.IsOutOfStock)
                    throw new Exception($"'{item.ProductName}' (Size: {item.Size}) is out of stock. Please remove it to continue.");
            }

            var subtotal = cartItems.Sum(c => c.ProductPrice * c.Quantity);
            var totalAmount = subtotal + ShippingCharge;

            var order = new Orders
            {
                UserId = userId,
                AddressId = dto.AddressId,  // ← ADD THIS
                OrderDateTime = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Status = OrderStatus.Pending
            };

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveAsync();

            foreach (var item in cartItems)
            {
                var orderItem = new OrderItems
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.ProductPrice,
                    ProductName = item.ProductName,
                    ProductImage = item.ProductImage,
                    Size = item.Size
                };

                await _orderItemsRepository.AddAsync(orderItem);

                var stock = await _stockRepository.FindAsync(s =>
                    s.ProductId == item.ProductId &&
                    s.Size == item.Size);

                if (stock != null)
                {
                    stock.Stock -= item.Quantity;
                    _stockRepository.Update(stock);
                }
            }

            await _orderItemsRepository.SaveAsync();
            await _stockRepository.SaveAsync();  // ← ADD THIS (was missing)

            var paymentStatus = dto.PaymentMethod == PaymentMethods.COD
                ? PaymentStatus.Pending
                : PaymentStatus.Success;

            var payment = new Payments
            {
                OrderId = order.Id,
                Amount = totalAmount,
                PaymentMethod = dto.PaymentMethod.ToString(),
                Status = paymentStatus,
                PaidAt = DateTime.UtcNow,
                TransactionId = Guid.NewGuid().ToString()
            };

            await _paymentRepository.AddAsync(payment);
            await _paymentRepository.SaveAsync();

            await _cartService.ClearAllCart(userId);

            return new UserOrderResponseDto
            {
                OrderId = order.Id,
                OrderDateTime = order.OrderDateTime,
                TotalAmount = order.TotalAmount,
                PaymentMethod = dto.PaymentMethod.ToString(),
                PaymentStatus = paymentStatus.ToString(),
                OrderStatus = order.Status.ToString()
            };
        }

        public async Task CancelOrderAsync(long orderId, long userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null || order.UserId != userId)
                throw new Exception("Order not found or you are not authorized.");

            if (order.Status == OrderStatus.Delivered)
                throw new Exception("Delivered orders cannot be cancelled.");

            if (order.Status == OrderStatus.Cancelled)
                throw new Exception("Order is already cancelled.");

            order.Status = OrderStatus.Cancelled;
            _orderRepository.Update(order);
            await _orderRepository.SaveAsync();
        }

        public async Task<List<UserOrderHistoryDto>> GetUserOrdersAsync(long userId)
        {
            var orders = await _orderRepository.FindAllAsync(o => o.UserId == userId);
            var orderIds = orders.Select(o => o.Id).ToList();
            var allItems = await _orderItemsRepository.FindAllAsync(i => orderIds.Contains(i.OrderId));

            return orders
                .OrderByDescending(o => o.OrderDateTime)
                .Select(o => new UserOrderHistoryDto
                {
                    OrderId = o.Id,
                    OrderedDate = o.OrderDateTime,
                    Status = o.Status.ToString(),
                    TotalAmount = o.TotalAmount,
                    Products = allItems
                        .Where(i => i.OrderId == o.Id)
                        .Select(i => new OrderItemDto
                        {
                            ProductId = i.ProductId,
                            ProductName = i.ProductName,
                            ProductImage = i.ProductImage,
                            Size = i.Size,
                            Quantity = i.Quantity,
                            Price = i.Price,
                            TotalPrice = i.Price * i.Quantity
                        }).ToList()
                }).ToList();
        }
    }
}

