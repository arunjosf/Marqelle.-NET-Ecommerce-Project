using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.DTO
{
    public class AddressCheckoutDto
    {
        public long AddressId { get; set; }
        public string FullName { get; set; }
        public string AddressLine { get; set; }
        public string CityStatePincode { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
    }
}
