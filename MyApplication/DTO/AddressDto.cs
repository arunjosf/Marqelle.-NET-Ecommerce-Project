using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.DTO
{
    public class AddressDto
    {
        [Required]
        public string AddressType { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Pincode { get; set; }
        [Required]
        public string FlatorHouseorBuildingName { get; set; }
        [Required]
        public string LandMark { get; set; }
    }
}
