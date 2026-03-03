using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Domain.Entities
{
    public class Cart
    {
        public long Id { get; set; }
        public long UserId  { get; set; }
        public Users User   { get; set; }
        public long ProductId { get; set; }
        public Products Product { get; set; }
        public int Quantity { get; set; }
        public string Size { get; set; }
    }
}
