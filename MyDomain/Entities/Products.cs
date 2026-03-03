using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Domain.Entities
{
    public class Products
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal price { get; set; }
        public double Rating    { get; set; }
        public string Color { get; set; }
        public bool InStock { get; set; }

        public long CategoryId { get; set; }
        public ProductsCategory Category { get; set; }

        public ICollection <ProductSize> Sizes { get; set; }
        public ICollection<ProductsImage> Images { get; set; }
        public ICollection<Cart> Carts  { get; set; }
        public ICollection<Wishlist> Wishlists { get; set; }
        public ICollection<OrderItems> OrderItems { get; set; }

    }
}
