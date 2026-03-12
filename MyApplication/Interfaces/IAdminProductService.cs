using Marqelle.Application.DTO;
using Marqelle.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.Interfaces
{
    public interface IAdminProductService
    {
        Task<List<string>> UploadImagesAsync(List<IFormFile> images, string webRootPath);
        Task AddProductAsync(AdminAddproductDto dto);
        Task UpdateProductAsync(long productId, AdminUpateProductDto dto);
        Task DeleteProductAsync(long productId);
        Task<List<ProductFetchingDto>> GetAllProductsAsync();
        Task<List<ProductFetchingDto>> SearchProductsAsync(long? id, string? name, string? color, string? category, decimal? price);

    }
}
