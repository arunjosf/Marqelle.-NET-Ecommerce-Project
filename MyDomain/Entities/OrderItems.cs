using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Domain.Entities
{
    public class OrderItems
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public Orders Order { get; set; }

        public long ProductId { get; set; }
        public Products Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; } 
    }
}
