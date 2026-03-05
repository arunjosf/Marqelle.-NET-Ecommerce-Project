using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Domain.Entities
{
    public class Address
    {
        public long Id {  get; set; }
        public long UserId { get; set; }
        public Users user { get; set; }
        [Required]
        public string AddressType { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Email {  get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Pincode { get; set; }
        [Required]
        public string FlatorHouseorBuildingName {  get; set; }
        
        public string LandMark { get; set; }

    }
}
