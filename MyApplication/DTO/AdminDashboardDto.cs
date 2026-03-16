using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.DTO
{
    public class TopSellingProductDto
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string CategoryName { get; set; }
        public int TotalSold { get; set; }
    }

    public class CategorySalesDto
    {
        public string CategoryName { get; set; }
        public decimal TotalSales { get; set; }
    }

    public class AdminDashboardDto
    {
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public int TotalUsers { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<TopSellingProductDto> TopSellingProducts { get; set; }
        public List<CategorySalesDto> SalesByCategory { get; set; }
    }
}
