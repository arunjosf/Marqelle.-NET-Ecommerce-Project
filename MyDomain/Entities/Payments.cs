using Marqelle.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Domain.Entities
{
    public class Payments
    {
            public long Id { get; set; }
            public long OrderId { get; set; }
            public Orders Order { get; set; }
            public decimal Amount { get; set; }
            public string PaymentMethod { get; set; }
            public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
            public DateTime PaidAt { get; set; } = DateTime.UtcNow;
            public string TransactionId { get; set; }
        }
    }

