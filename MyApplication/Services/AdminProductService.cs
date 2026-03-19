using Marqelle.Application.DTO;
using Marqelle.Application.Interfaces;
using Marqelle.Domain.Entities;
using Microsoft.AspNetCore.Http;
namespace Marqelle.Application.Services
{
    public class AdminProductService : IAdminProductService
    {
        private readonly IGenericRepository<Products> _productRepository;
        private readonly IGenericRepository<ProductsCategory> _categoryRepository;
        private readonly IGenericRepository<ProductsImage> _imageRepository;
        private readonly IGenericRepository<ProductSizeAndStock> _stockRepository;
        private readonly ICloudinaryService _cloudinaryService;

        private static readonly string[] ValidSizes = { "S", "M", "L", "XL" };

        public AdminProductService(
            IGenericRepository<Products> productRepository,
            IGenericRepository<ProductsCategory> categoryRepository,
            IGenericRepository<ProductsImage> imageRepository,
            IGenericRepository<ProductSizeAndStock> stockRepository,
            ICloudinaryService cloudinaryService)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _imageRepository = imageRepository;
            _stockRepository = stockRepository;
            _cloudinaryService = cloudinaryService;
        }

        private void ValidateSizes(List<SizeStockDto> sizes)
        {
            foreach (var sizeStock in sizes)
            {
                if (!ValidSizes.Contains(sizeStock.Size.ToUpper()))
                    throw new Exception($"Invalid size '{sizeStock.Size}'. Must be one of: {string.Join(", ", ValidSizes)}.");

                if (sizeStock.Stock <= 0)
                    throw new Exception($"Stock for size '{sizeStock.Size}' must be greater than 0.");
            }

            var duplicates = sizes
                .GroupBy(s => s.Size.ToUpper())
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Any())
                throw new Exception($"Duplicate sizes found: {string.Join(", ", duplicates)}.");
        }

        public async Task<long> AddProductAsync(AdminAddproductDto dto)
        {
            var category = await _categoryRepository.FindAsync(c =>
                c.Name.ToLower() == dto.Category.ToLower());

            if (category == null)
            {
                category = new ProductsCategory { Name = dto.Category };
                await _categoryRepository.AddAsync(category);
                await _categoryRepository.SaveAsync();
            }

            var product = new Products
            {
                Name = dto.Name,
                price = dto.Price,
                Color = dto.Color,
                Description = dto.Description,
                Rating = dto.Rating,
                CategoryId = category.Id
            };

            await _productRepository.AddAsync(product);
            await _productRepository.SaveAsync();

            foreach (var file in dto.Images)
            {
                var url = await _cloudinaryService.UploadImageAsync(file);

                await _imageRepository.AddAsync(new ProductsImage
                {
                    ProductId = product.Id,
                    ImageUrl = url
                });
            }

            await _imageRepository.SaveAsync();

            return product.Id;
        }

        public async Task AddStockAsync(long productId, AddStockDto dto)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
                throw new Exception("Product not found.");

            ValidateSizes(dto.Sizes);

            var existingStocks = await _stockRepository.FindAllAsync(s => s.ProductId == productId);

            foreach (var sizeStock in dto.Sizes)
            {
                var size = sizeStock.Size.ToUpper();
                var existing = existingStocks.FirstOrDefault(s => s.Size == size);

                if (existing != null)
                {
                    existing.Stock += sizeStock.Stock;
                    _stockRepository.Update(existing);
                }
                else
                {
                    await _stockRepository.AddAsync(new ProductSizeAndStock
                    {
                        ProductId = productId,
                        Size = size,
                        Stock = sizeStock.Stock
                    });
                }
            }

            await _stockRepository.SaveAsync();
        }

        public async Task UpdateProductAsync(long productId, AdminUpateProductDto dto)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
                throw new Exception("Product not found.");

            if (!string.IsNullOrEmpty(dto.Name)) product.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Color)) product.Color = dto.Color;
            if (!string.IsNullOrEmpty(dto.Description)) product.Description = dto.Description;
            if (dto.Price.HasValue) product.price = dto.Price.Value;
            if (dto.Rating.HasValue) product.Rating = dto.Rating.Value;

            if (!string.IsNullOrEmpty(dto.Category))
            {
                var category = await _categoryRepository.FindAsync(c =>
                    c.Name.ToLower() == dto.Category.ToLower());

                if (category == null)
                {
                    category = new ProductsCategory { Name = dto.Category };
                    await _categoryRepository.AddAsync(category);
                    await _categoryRepository.SaveAsync();
                }

                product.CategoryId = category.Id;
            }

            _productRepository.Update(product);
            await _productRepository.SaveAsync();

            if (dto.Images != null && dto.Images.Any())
            {
                var existingImages = await _imageRepository.FindAllAsync(i => i.ProductId == productId);

                foreach (var img in existingImages)
                {
                    await _cloudinaryService.DeleteImageAsync(img.ImageUrl);
                    _imageRepository.Delete(img);
                }

                await _imageRepository.SaveAsync();

                foreach (var file in dto.Images)
                {
                    var url = await _cloudinaryService.UploadImageAsync(file);

                    await _imageRepository.AddAsync(new ProductsImage
                    {
                        ProductId = productId,
                        ImageUrl = url
                    });
                }

                await _imageRepository.SaveAsync();
            }
        }

        public async Task UpdateStockAsync(long productId, AddStockDto dto)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
                throw new Exception("Product not found.");

            ValidateSizes(dto.Sizes);

            var existingStocks = await _stockRepository.FindAllAsync(s => s.ProductId == productId);
            foreach (var stock in existingStocks)
                _stockRepository.Delete(stock);

            await _stockRepository.SaveAsync();

            foreach (var sizeStock in dto.Sizes)
            {
                await _stockRepository.AddAsync(new ProductSizeAndStock
                {
                    ProductId = productId,
                    Size = sizeStock.Size.ToUpper(),
                    Stock = sizeStock.Stock
                });
            }

            await _stockRepository.SaveAsync();
        }

        public async Task DeleteProductAsync(long productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
                throw new Exception("Product not found.");

            if (product.IsDeleted)
                throw new Exception("Product is already deleted.");

            product.IsDeleted = true;
            _productRepository.Update(product);
            await _productRepository.SaveAsync();
        }

        public async Task<List<ProductFetchingDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync(
                p => p.Category,
                p => p.Images,
                p => p.Stocks);

            return products.Where(p => !p.IsDeleted).Select(MapToDto).ToList();
        }

        public async Task<List<ProductFetchingDto>> SearchProductsAsync(
            long? id, string? name, string? color, string? category, decimal? price)
        {
            var products = await _productRepository.GetAllAsync(
                p => p.Category,
                p => p.Images,
                p => p.Stocks);

            var query = products.Where(p => !p.IsDeleted).AsQueryable();

            if (id.HasValue)
                query = query.Where(p => p.Id == id.Value);

            if (!string.IsNullOrEmpty(name))
                query = query.Where(p => p.Name.ToLower().Contains(name.ToLower()));

            if (!string.IsNullOrEmpty(color))
                query = query.Where(p => p.Color.ToLower().Contains(color.ToLower()));

            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => p.Category.Name.ToLower().Contains(category.ToLower()));

            if (price.HasValue)
                query = query.Where(p => p.price <= price.Value);

            return query.Select(MapToDto).ToList();
        }

        private static ProductFetchingDto MapToDto(Products p) => new ProductFetchingDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.price,
            Color = p.Color,
            Rating = p.Rating,
            CategoryName = p.Category?.Name ?? string.Empty,
            StockInfo = (p.Stocks != null && p.Stocks.Any() && p.Stocks.All(s => s.Stock == 0))
                           ? "Temporarily unavailable" : null,
            Sizes = p.Stocks != null
                           ? p.Stocks.Where(s => s.Stock > 0).Select(s => s.Size).ToList()
                           : new List<string>(),
            SizeStocks = p.Stocks != null
                           ? p.Stocks.Select(s => new SizeStockInfoDto { Size = s.Size, Stock = s.Stock }).ToList()
                           : new List<SizeStockInfoDto>(),
            Images = p.Images?.Select(i => i.ImageUrl).ToList() ?? new List<string>()
        };
    }
}