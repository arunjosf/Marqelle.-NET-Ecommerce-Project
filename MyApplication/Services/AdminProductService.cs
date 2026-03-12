using Marqelle.Application.DTO;
using Marqelle.Application.Interfaces;
using Marqelle.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Marqelle.Application.Services
{
    public class AdminProductService : IAdminProductService
    {
        private readonly IGenericRepository<Products> _productRepository;
        private readonly IGenericRepository<ProductsCategory> _categoryRepository;
        private readonly IGenericRepository<ProductsImage> _imageRepository;
        private readonly IGenericRepository<ProductSizeAndStock> _stockRepository;

        private static readonly string[] ValidSizes = { "XS", "S", "M", "L", "XL" };

        public AdminProductService(
            IGenericRepository<Products> productRepository,
            IGenericRepository<ProductsCategory> categoryRepository,
            IGenericRepository<ProductsImage> imageRepository,
            IGenericRepository<ProductSizeAndStock> stockRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _imageRepository = imageRepository;
            _stockRepository = stockRepository;
        }

        // Step 1 — Upload images, return URLs
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

        // Step 2 — Create product with URLs from step 1
        public async Task AddProductAsync(AdminAddproductDto dto)
        {
            // 1. Validate sizes
            foreach (var sizeStock in dto.Sizes)
            {
                if (!ValidSizes.Contains(sizeStock.Size.ToUpper()))
                    throw new Exception($"Invalid size '{sizeStock.Size}'. Must be one of: {string.Join(", ", ValidSizes)}.");

                if (sizeStock.Stock <= 0)
                    throw new Exception($"Stock for size '{sizeStock.Size}' must be greater than 0.");
            }

            // 2. Check duplicate sizes
            var duplicates = dto.Sizes
                .GroupBy(s => s.Size.ToUpper())
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Any())
                throw new Exception($"Duplicate sizes found: {string.Join(", ", duplicates)}.");

            // 3. Get or create category
            var category = await _categoryRepository.FindAsync(c =>
                c.Name.ToLower() == dto.Category.ToLower());

            if (category == null)
            {
                category = new ProductsCategory { Name = dto.Category };
                await _categoryRepository.AddAsync(category);
                await _categoryRepository.SaveAsync();
            }

            // 4. Create product
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

            // 5. Save image URLs
            foreach (var url in dto.ImageUrls)
            {
                await _imageRepository.AddAsync(new ProductsImage
                {
                    ProductId = product.Id,
                    ImageUrl = url
                });
            }

            await _imageRepository.SaveAsync();

            // 6. Save sizes and stock
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

            // Update basic fields if provided
            if (!string.IsNullOrEmpty(dto.Name)) product.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Color)) product.Color = dto.Color;
            if (!string.IsNullOrEmpty(dto.Description)) product.Description = dto.Description;
            if (dto.Price.HasValue) product.price = dto.Price.Value;
            if (dto.Rating.HasValue) product.Rating = dto.Rating.Value;

            // Update category if provided
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

            // Replace images if provided
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

            // Replace sizes and stocks if provided
            if (dto.Sizes != null && dto.Sizes.Any())
            {
                // Validate new sizes
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

        // Search products by id, name, color, category, price — all optional
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

        // Delete product and all related data
        public async Task DeleteProductAsync(long productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
                throw new Exception("Product not found.");

            // Delete images
            var images = await _imageRepository.FindAllAsync(i => i.ProductId == productId);
            foreach (var img in images)
                _imageRepository.Delete(img);

            await _imageRepository.SaveAsync();

            // Delete sizes and stocks
            var stocks = await _stockRepository.FindAllAsync(s => s.ProductId == productId);
            foreach (var stock in stocks)
                _stockRepository.Delete(stock);

            await _stockRepository.SaveAsync();

            // Delete product
            _productRepository.Delete(product);
            await _productRepository.SaveAsync();
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


