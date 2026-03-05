using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.DTO
{
    public class UserCartDto
    {
        public long CartId { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal ProductPrice { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => ProductPrice * Quantity;
        public decimal TotalCartPrice { get; set; }
        public int TotalCartQuantity { get; set; }
    }
}
