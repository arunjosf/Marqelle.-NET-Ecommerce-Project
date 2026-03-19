using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.DTO
{


    public class SizeStockInfoDto
    {
        public string Size { get; set; }
        public int Stock { get; set; }
    }
    public class ProductFetchingDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Color { get; set; }
        public string? StockInfo { get; set; }
        public string CategoryName { get; set; }
        public List<string> Sizes { get; set; }
        public List<SizeStockInfoDto> SizeStocks { get; set; }
        public List<string> Images { get; set; }
        public double Rating { get; set; }
    }
}
