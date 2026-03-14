using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    namespace Marqelle.Application.DTO
    {
        public class AdminUpateProductDto
        {
            public string? Name { get; set; }

            [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
            public decimal? Price { get; set; }

            public string? Color { get; set; }

            public string? Category { get; set; }

            public string? Description { get; set; }

            [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5.")]
            public double? Rating { get; set; }

            public List<IFormFile>? Images { get; set; }
        }
    }

