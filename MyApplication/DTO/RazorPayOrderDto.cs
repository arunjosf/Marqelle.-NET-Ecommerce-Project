using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.DTO
{
    public class RazorPayOrderDto
    {
        public string RazorpayOrderId { get; set; }
        public long Amount { get; set; }
        public string Currency { get; set; }
        public string KeyId { get; set; }
    }
}
