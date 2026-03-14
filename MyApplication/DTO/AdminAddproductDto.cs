using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.DTO
{

    public class SizeStockDto
    {
        [Required(ErrorMessage = "Size is required.")]
        public string Size { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Stock must be greater than 0.")]
        public int Stock { get; set; }
    }

    public class AdminAddproductDto
    {
        [Required(ErrorMessage = "Enter product name.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter product price.")]
        [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Enter product color.")]
        public string Color { get; set; }

        [Required(ErrorMessage = "Enter category.")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Enter description.")]
        public string Description { get; set; }

        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5.")]
        public double Rating { get; set; }

        [Required(ErrorMessage = "Add at least one image.")]
        [MinLength(1, ErrorMessage = "Add at least one image.")]
        public List<IFormFile> Images { get; set; } = new();
    }

    public class AddStockDto
    {
        [Required(ErrorMessage = "Add at least one size with stock.")]
        [MinLength(1, ErrorMessage = "Add at least one size with stock.")]
        public List<SizeStockDto> Sizes { get; set; } = new();
    }
}
