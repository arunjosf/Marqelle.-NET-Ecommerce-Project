using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Domain.Entities
{
    public class ProductsCategory
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ICollection<Products> Product { get; set; }
    }
}
