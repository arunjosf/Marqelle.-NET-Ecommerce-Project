using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Domain.Entities
{
    public class ProductSizeAndStock
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string Size { get; set; }  
        public int Stock { get; set; }       
        public Products Product { get; set; }
    }
}
