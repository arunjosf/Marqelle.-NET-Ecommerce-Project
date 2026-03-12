using Marqelle.Application.DTO;
using Marqelle.Application.Interfaces;
using Marqelle.Domain.Entities;
using Marqelle.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.Services
{
    public class AdminOrderService : IAdminOrderService
    {
        private readonly IGenericRepository<Orders> _orderRepository;
        private readonly IGenericRepository<OrderItems> _orderItemsRepository;
        private readonly IGenericRepository<Payments> _paymentRepository;

        public AdminOrderService(IGenericRepository<Orders> orderRepository, IGenericRepository<OrderItems> orderItemsRepository,
            IGenericRepository<Payments> paymentRepository)
        {
            _orderRepository = orderRepository;
            _orderItemsRepository = orderItemsRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task UpdateOrderStatusAsync(long orderId, OrderStatus newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
                throw new Exception("Order not found.");

            if (order.Status == OrderStatus.Cancelled)
                throw new Exception("Cancelled orders cannot be updated.");

            if (order.Status == OrderStatus.Delivered)
                throw new Exception("Delivered orders cannot be updated.");

            if (newStatus == OrderStatus.Cancelled)
                throw new Exception("Admin cannot cancel an order");

            order.Status = newStatus;
            _orderRepository.Update(order);
            await _orderRepository.SaveAsync();
        }

        public async Task<List<AdminOrderHistoryDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            var orderIds = orders.Select(o => o.Id).ToList();

            var allItems = await _orderItemsRepository.FindAllAsync(i => orderIds.Contains(i.OrderId));
            var allPayments = await _paymentRepository.FindAllAsync(p => orderIds.Contains(p.OrderId));

            return orders
                .OrderByDescending(o => o.OrderDateTime)
                .Select(o => new AdminOrderHistoryDto
                {
                    OrderId = o.Id,
                    UserId = o.UserId,
                    OrderedDate = o.OrderDateTime,
                    Status = o.Status.ToString(),
                    TotalAmount = o.TotalAmount,
                    PaymentMethod = allPayments
                        .FirstOrDefault(p => p.OrderId == o.Id)?.PaymentMethod ?? "N/A",
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

        public async Task<List<AdminOrderHistoryDto>> SearchOrdersAsync(long? orderId, long? productId, long? userId)
        {
            var allOrders = await _orderRepository.GetAllAsync();
            var orders = allOrders.AsEnumerable();

            if (orderId.HasValue)
                orders = orders.Where(o => o.Id == orderId.Value);

            if (userId.HasValue)
                orders = orders.Where(o => o.UserId == userId.Value);

            var filteredOrders = orders.ToList();

            if (!filteredOrders.Any())
                return new List<AdminOrderHistoryDto>();

            var orderIds = filteredOrders.Select(o => o.Id).ToList();

            var allItems = await _orderItemsRepository.FindAllAsync(i => orderIds.Contains(i.OrderId));

            if (productId.HasValue)
            {
                var matchingOrderIds = allItems
                    .Where(i => i.ProductId == productId.Value)
                    .Select(i => i.OrderId)
                    .Distinct()
                    .ToList();

                filteredOrders = filteredOrders.Where(o => matchingOrderIds.Contains(o.Id)).ToList();
                allItems = allItems.Where(i => matchingOrderIds.Contains(i.OrderId)).ToList();
            }

            var allPayments = await _paymentRepository.FindAllAsync(p => orderIds.Contains(p.OrderId));

            return filteredOrders
                .OrderByDescending(o => o.OrderDateTime)
                .Select(o => new AdminOrderHistoryDto
                {
                    OrderId = o.Id,
                    UserId = o.UserId,
                    OrderedDate = o.OrderDateTime,
                    Status = o.Status.ToString(),
                    TotalAmount = o.TotalAmount,
                    PaymentMethod = allPayments.FirstOrDefault(p => p.OrderId == o.Id)?.PaymentMethod ?? "N/A",
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

