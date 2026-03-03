using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Domain.Entities
{
    public class ProductsImage
    {
        public long Id { get; set; }
        public string ImageUrl { get; set; }

        public long ProductId { get; set; }
        public Products Product { get; set; }

    }
}
