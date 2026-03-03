using Marqelle.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Marqelle.Domain.Entities.Payments;

namespace Marqelle.Domain.Entities
{
    public class Orders
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTime OrderDateTime { get; set; }
        public decimal TotalAmount { get; set; }
        public ICollection<OrderItems> OrderItems { get; set; }
        public ICollection<Payments> Payment { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
    }
}

