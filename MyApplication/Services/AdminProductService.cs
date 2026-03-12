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

        private static readonly string[] ValidSizes = { "XS", "S", "M", "L", "XL" };

        public AdminProductService(IGenericRepository<Products> productRepository,IGenericRepository<ProductsCategory> categoryRepository,
            IGenericRepository<ProductsImage> imageRepository,
            IGenericRepository<ProductSizeAndStock> stockRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _imageRepository = imageRepository;
            _stockRepository = stockRepository;
        }

        public async Task<List<string>> UploadImagesAsync(List<IFormFile> images, string webRootPath)
        {
            if (images == null || !images.Any())
                throw new Exception("Add at least one image.");

            var uploadFolder = Path.Combine(webRootPath, "images", "products");
            Directory.CreateDirectory(uploadFolder);

            var imageUrls = new List<string>();

            foreach (var file in images)
            {
                if (file.Length == 0) continue;

                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                imageUrls.Add($"/images/products/{fileName}");
            }

            return imageUrls;
        }

        public async Task AddProductAsync(AdminAddproductDto dto)
        {
            foreach (var sizeStock in dto.Sizes)
            {
                if (!ValidSizes.Contains(sizeStock.Size.ToUpper()))
                    throw new Exception($"Invalid size '{sizeStock.Size}'. Must be one of: {string.Join(", ", ValidSizes)}.");

                if (sizeStock.Stock <= 0)
                    throw new Exception($"Stock for size '{sizeStock.Size}' must be greater than 0.");
            }

            var duplicates = dto.Sizes
                .GroupBy(s => s.Size.ToUpper())
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Any())
                throw new Exception($"Duplicate sizes found: {string.Join(", ", duplicates)}.");

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

            foreach (var url in dto.ImageUrls)
            {
                await _imageRepository.AddAsync(new ProductsImage
                {
                    ProductId = product.Id,
                    ImageUrl = url
                });
            }

            await _imageRepository.SaveAsync();

            foreach (var sizeStock in dto.Sizes)
            {
                await _stockRepository.AddAsync(new ProductSizeAndStock
                {
                    ProductId = product.Id,
                    Size = sizeStock.Size.ToUpper(),
                    Stock = sizeStock.Stock
                });
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

            if (dto.ImageUrls != null && dto.ImageUrls.Any())
            {
                var existingImages = await _imageRepository.FindAllAsync(i => i.ProductId == productId);
                foreach (var img in existingImages)
                    _imageRepository.Delete(img);

                await _imageRepository.SaveAsync();

                foreach (var url in dto.ImageUrls)
                {
                    await _imageRepository.AddAsync(new ProductsImage
                    {
                        ProductId = productId,
                        ImageUrl = url
                    });
                }

                await _imageRepository.SaveAsync();
            }

            if (dto.Sizes != null && dto.Sizes.Any())
            {

                foreach (var sizeStock in dto.Sizes)
                {
                    if (!ValidSizes.Contains(sizeStock.Size.ToUpper()))
                        throw new Exception($"Invalid size '{sizeStock.Size}'. Must be one of: {string.Join(", ", ValidSizes)}.");

                    if (sizeStock.Stock <= 0)
                        throw new Exception($"Stock for size '{sizeStock.Size}' must be greater than 0.");
                }

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
        }

        public async Task<List<ProductFetchingDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync(
                p => p.Category,
                p => p.Images,
                p => p.Stocks);

            return products.Select(MapToDto).ToList();
        }

        public async Task<List<ProductFetchingDto>> SearchProductsAsync(
            long? id, string? name, string? color, string? category, decimal? price)
        {
            var products = await _productRepository.GetAllAsync(
                p => p.Category,
                p => p.Images,
                p => p.Stocks);

            var query = products.AsQueryable();

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

        public async Task DeleteProductAsync(long productId, string webRootPath)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
                throw new Exception("Product not found.");

            var images = await _imageRepository.FindAllAsync(i => i.ProductId == productId);
            foreach (var img in images)
            {
                DeleteImageFile(img.ImageUrl, webRootPath);
                _imageRepository.Delete(img);
            }

            await _imageRepository.SaveAsync();

            var stocks = await _stockRepository.FindAllAsync(s => s.ProductId == productId);
            foreach (var stock in stocks)
                _stockRepository.Delete(stock);

            await _stockRepository.SaveAsync();

            _productRepository.Delete(product);
            await _productRepository.SaveAsync();
        }

        private void DeleteImageFile(string imageUrl, string webRootPath)
        {
            try
            {
                var filePath = Path.Combine(webRootPath, imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch {
            }
        }

        private static ProductFetchingDto MapToDto(Products p) => new ProductFetchingDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.price,
            Color = p.Color,

            StockInfo = (p.Stocks != null && p.Stocks.Any() && p.Stocks.All(s => s.Stock == 0))
           ? "Temporarily unavailable"
           : null,

            Rating = p.Rating,
            CategoryName = p.Category?.Name ?? string.Empty,

            Sizes = (p.Stocks != null && p.Stocks.Any())
           ? p.Stocks.Where(s => s.Stock > 0).Select(s => s.Size).ToList()
           : new List<string>(),

            Images = p.Images?.Select(i => i.ImageUrl).ToList() ?? new List<string>()
        };
    }
}


