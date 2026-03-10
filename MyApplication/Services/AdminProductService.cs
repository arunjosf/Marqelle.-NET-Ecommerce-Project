//using Marqelle.Application.DTO;
//using Marqelle.Application.Interfaces;
//using Marqelle.Domain.Entities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Marqelle.Application.Services
//{
//    public class AdminProductService : IAdminProductService
//    {
//        private readonly IAdminProductRepository _repository;

//        public AdminProductService(IAdminProductRepository repository)
//        {
//            _repository = repository;
//        }

//        public async Task<Products> AddProducts(AdminAddproductDto dto)
//        {
//            if (dto.Sizes == null || !dto.Sizes.Any())
//                throw new ArgumentException("Select at least one size.");

//            if (dto.ImageUrls == null || !dto.ImageUrls.Any())
//                throw new ArgumentException("Add at least one image.");

//            var product = new Products
//            {
//                Name = dto.Name,
//                price = dto.Price,
//                Color = dto.Color,
//                Description = dto.Description,
//                Stocks = dto.Stocks.Select(s => new ProductStock { Stock = s}).ToList(),
//                Sizes = dto.Sizes.Select(s => new ProductSize { Size = s }).ToList(),
//                Images = dto.ImageUrls.Select(url => new ProductsImage { ImageUrl = url }).ToList(),
//                Rating = dto.Rating
//            };

//            return await _repository.AddProductsAsync(product, dto.Category);
//        }
//    }
//}
