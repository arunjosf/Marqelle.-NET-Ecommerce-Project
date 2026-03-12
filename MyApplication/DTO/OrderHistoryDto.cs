using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.DTO
{
    public class OrderItemDto
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class UserOrderHistoryDto
    {
        public long OrderId { get; set; }
        public DateTime OrderedDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemDto> Products { get; set; }
    }

    public class AdminOrderHistoryDto
    {
        public long OrderId { get; set; }
        public long UserId { get; set; }
        public DateTime OrderedDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public List<OrderItemDto> Products { get; set; }
    }
}
