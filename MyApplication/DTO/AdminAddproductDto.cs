using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.DTO
{
    public class AdminAddproductDto
    {
        [Required(ErrorMessage = "Enter product name.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter product price.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Enter product color.")]
        public string Color { get; set; }

        [Required(ErrorMessage = "Enter category.")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Select at least one size.")]
        [MinLength(1, ErrorMessage = "Select at least one size.")]
        public List<string> Sizes { get; set; } = new List<string>();

        [Required(ErrorMessage = "Add at least one image.")]
        [MinLength(1, ErrorMessage = "Add at least one image.")]
        public List<string> ImageUrls { get; set; } = new List<string>();
        [Required(ErrorMessage = "Enter Description.")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Enter Rating.")]
        public double Rating { get; set; }
        [Required(ErrorMessage = "Enter Stock")]
        public List<int> Stocks { get; set; } = new List<int>();
    }
}
