using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;
using TiendaUCN.Domain.Models.Product;
using TiendaUCN.Infrastructure.Data.Repository;
using TiendaUCN.src.Application.Services.Interfaces;
using TiendaUCN.src.Domain.Models;

namespace TiendaUCN.Application.Services.Implements
{
    public class ImageService : IImageService
    {
        private readonly Cloudinary? _cloudinary;
        private readonly IImageRepository _imageRepository;

        public ImageService(IConfiguration configuration, IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;

            var cloudName = configuration["CLOUDINARY_CLOUD_NAME"]
                ?? configuration["Cloudinary:CloudName"];
            var apiKey = configuration["CLOUDINARY_API_KEY"]
                ?? configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["CLOUDINARY_API_SECRET"]
                ?? configuration["Cloudinary:ApiSecret"];

            if (!string.IsNullOrWhiteSpace(cloudName)
                && !string.IsNullOrWhiteSpace(apiKey)
                && !string.IsNullOrWhiteSpace(apiSecret))
            {
                var account = new Account(cloudName, apiKey, apiSecret);
                _cloudinary = new Cloudinary(account);
            }
        }

        public async Task<bool> UploadImageAsync(IFormFile file, int productId)
        {
            if (file == null || file.Length == 0) return false;

            string? imageUrl = null;
            string publicId = Guid.NewGuid().ToString();

            // 1. Intentar subir a Cloudinary
            if (_cloudinary != null)
            {
                try
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(file.FileName, stream),
                            Transformation = new Transformation().Width(500).Height(500).Crop("fill")
                        };

                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        if (uploadResult != null && uploadResult.Error == null && uploadResult.SecureUrl != null)
                        {
                            imageUrl = uploadResult.SecureUrl.ToString();
                            publicId = uploadResult.PublicId ?? publicId;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "No se pudo subir la imagen a Cloudinary. Se procederá a guardarla localmente.");
                }
            }

            // 2. Si Cloudinary no generó URL (error de API o no configurado), guardar la imagen localmente en wwwroot/uploads
            if (string.IsNullOrEmpty(imageUrl))
            {
                try
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    imageUrl = $"http://localhost:5094/uploads/{uniqueFileName}";
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error al guardar la imagen localmente para el producto {ProductId}", productId);
                    return false;
                }
            }

            // 3. Persistir en la base de datos
            var newImage = new Image
            {
                ImageUrl = imageUrl,
                PublicId = publicId,
                ProductId = productId
            };

            var result = await _imageRepository.CreateImageAsync(newImage);
            return result ?? false;
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            if (_cloudinary is null)
            {
                throw new InvalidOperationException("Cloudinary no esta configurado. Define CLOUDINARY_CLOUD_NAME, CLOUDINARY_API_KEY y CLOUDINARY_API_SECRET.");
            }

            if (string.IsNullOrEmpty(publicId)) return false;

            var deletionParams = new DeletionParams(publicId);
            var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

            if (deletionResult.Result != "ok") return false;

            var result = await _imageRepository.DeleteAsync(publicId);
            return result ?? false;
        }
    }
}