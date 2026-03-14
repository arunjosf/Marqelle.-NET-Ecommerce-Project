using Marqelle.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Infrastructure.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration configuration)
        {
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "marqelle/products",
                UseFilename = false,
                UniqueFilename = true
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
                throw new Exception($"Image upload failed: {result.Error.Message}");

            return result.SecureUrl.ToString();
        }

        public async Task DeleteImageAsync(string imageUrl)
        {
            try
            {
       
                https://res.cloudinary.com/cloudname/image/upload/v123456/marqelle/products/filename
                var uri = new Uri(imageUrl);
                var segments = uri.AbsolutePath.Split('/');
                var uploadIdx = Array.IndexOf(segments, "upload");

                if (uploadIdx < 0) return;

                var pathParts = segments.Skip(uploadIdx + 1)
                    .SkipWhile(s => s.StartsWith("v") && s.Length > 1 && s.Skip(1).All(char.IsDigit))
                    .ToArray();

                var publicId = string.Join("/", pathParts);

                var dotIndex = publicId.LastIndexOf('.');
                if (dotIndex > 0)
                    publicId = publicId.Substring(0, dotIndex);

                var deleteParams = new DeletionParams(publicId);
                await _cloudinary.DestroyAsync(deleteParams);
            }
            catch { }
        }
    }
}
