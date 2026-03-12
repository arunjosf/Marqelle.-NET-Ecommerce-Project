using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marqelle.Domain.Enums;


namespace Marqelle.Application.DTO
{
     public class PlaceOrderDto
    {
        [Required(ErrorMessage = "Please select a delivery address.")]
        public long AddressId { get; set; }

        [Required(ErrorMessage = "Please select a payment method.")]
        public PaymentMethods PaymentMethod { get; set; }
    }
}
