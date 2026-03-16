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
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IGenericRepository<Orders> _orderRepository;
        private readonly IGenericRepository<OrderItems> _orderItemsRepository;
        private readonly IGenericRepository<Products> _productRepository;
        private readonly IGenericRepository<Users> _userRepository;
        private readonly IGenericRepository<ProductsCategory> _categoryRepository;

        public AdminDashboardService(
            IGenericRepository<Orders> orderRepository,
            IGenericRepository<OrderItems> orderItemsRepository,
            IGenericRepository<Products> productRepository,
            IGenericRepository<Users> userRepository,
            IGenericRepository<ProductsCategory> categoryRepository)
        {
            _orderRepository = orderRepository;
            _orderItemsRepository = orderItemsRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<AdminDashboardDto> GetDashboardDataAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            var orderItems = await _orderItemsRepository.GetAllAsync();
            var products = await _productRepository.GetAllAsync(p => p.Category, p => p.Images);
            var users = await _userRepository.GetAllAsync();

            var activeProducts = products.Where(p => !p.IsDeleted).ToList();

            var totalUsers = users.Count(u => u.RoleId == 1);

            var deliveredOrders = orders.Where(o => o.Status == OrderStatus.Delivered).ToList();
            var deliveredOrderIds = deliveredOrders.Select(o => o.Id).ToList();
            var deliveredItems = orderItems.Where(i => deliveredOrderIds.Contains(i.OrderId)).ToList();

            var totalRevenue = deliveredOrders.Sum(o => o.TotalAmount);

            var soldMap = deliveredItems
                .GroupBy(i => i.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalSold = g.Sum(i => i.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(5)
                .ToList();

            var topSellingProducts = soldMap.Select(s =>
            {
                var product = activeProducts.FirstOrDefault(p => p.Id == s.ProductId);
                return new TopSellingProductDto
                {
                    ProductId = s.ProductId,
                    ProductName = product?.Name ?? "Unknown",
                    ProductImage = product?.Images?.FirstOrDefault()?.ImageUrl ?? "",
                    CategoryName = product?.Category?.Name ?? "N/A",
                    TotalSold = s.TotalSold
                };
            }).ToList();

            var salesByCategory = deliveredItems
                .GroupBy(i =>
                {
                    var product = activeProducts.FirstOrDefault(p => p.Id == i.ProductId);
                    return product?.Category?.Name ?? "Other";
                })
                .Select(g => new CategorySalesDto
                {
                    CategoryName = g.Key,
                    TotalSales = g.Sum(i => i.Price * i.Quantity)
                })
                .OrderByDescending(c => c.TotalSales)
                .ToList();

            return new AdminDashboardDto
            {
                TotalOrders = orders.Count(),
                TotalProducts = activeProducts.Count,
                TotalUsers = totalUsers,
                TotalRevenue = totalRevenue,
                TopSellingProducts = topSellingProducts,
                SalesByCategory = salesByCategory
            };
        }
    }
}
