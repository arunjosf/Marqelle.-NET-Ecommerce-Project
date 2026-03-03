using Marqelle.Application.DTO;
using Marqelle.Domain.Entities;
using Marqelle.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.Interfaces
{
    public interface IUserProductService
    {
        Task<List<ProductFetchingDto>> GetAllProducts();

        Task<List<ProductFetchingDto>> SearchProducts(string? name, string color,
            string? category, decimal? price, ProductSortPrice pricesort = ProductSortPrice.None,
            ProductSortRating ratingsort = ProductSortRating.None, ProductSortCategory? categoryFilter = ProductSortCategory.None);
        Task<ProductFetchingDto> GetProductById(long productId);
    }
}
