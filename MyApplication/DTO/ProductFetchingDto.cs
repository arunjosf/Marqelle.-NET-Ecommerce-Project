using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.DTO
{
    public class ProductFetchingDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Color { get; set; }
        public bool InStock { get; set; }
        public string CategoryName { get; set; }
        public List<string> Sizes { get; set; }
        public List<string> Images { get; set; }
        public double Rating { get; set; }
    }
}
