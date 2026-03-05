using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.DTO
{
    public class CheckoutDto
    {
        public List<CheckoutProductDto> Products { get; set; }
        public List<AddressCheckoutDto> Addresses { get; set; }
        public decimal SubTotal { get; set; }
        public decimal ShippingCharge { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
