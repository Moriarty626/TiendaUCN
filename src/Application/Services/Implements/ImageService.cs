using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using TiendaUCN.Domain.Models.Product;
using TiendaUCN.Infrastructure.Data.Repository;
using TiendaUCN.src.Application.Services.Interfaces;
using TiendaUCN.src.Domain.Models;

namespace TiendaUCN.Application.Services.Implements
{
    public class ImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly IImageRepository _imageRepository;

        public ImageService(IConfiguration configuration, IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;

            // ✅ Credenciales configuradas directamente como strings
            // NOTA: Asegúrate de que "Root" sea exactamente el Cloud Name de tu consola de Cloudinary
            var account = new Account(
                configuration["CLOUDINARY_CLOUD_NAME"],
                configuration["CLOUDINARY_API_KEY"],
                configuration["CLOUDINARY_API_SECRET"]
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<bool> UploadImageAsync(IFormFile file, int productId)
        {
            if (file == null || file.Length == 0) return false;

            var uploadResult = new ImageUploadResult();

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Width(500).Height(500).Crop("fill")
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            if (uploadResult.Error != null)
            {
                throw new Exception("Error al subir imagen a Cloudinary: " + uploadResult.Error.Message);
            }

            // 2. Crear el objeto Image para la base de datos
            // Asegúrate de que los nombres de las propiedades (ImageUrl, PublicId, ProductId) 
            // coincidan con tu modelo en Domain.Models
            var newImage = new Image
            {
                ImageUrl = uploadResult.SecureUrl.ToString(),
                PublicId = uploadResult.PublicId,
                ProductId = productId
            };

            // 3. Guardar en la base de datos
            var result = await _imageRepository.CreateImageAsync(newImage);

            return result ?? false;
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            if (string.IsNullOrEmpty(publicId)) return false;

            var deletionParams = new DeletionParams(publicId);
            var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

            if (deletionResult.Result != "ok") return false;

            var result = await _imageRepository.DeleteAsync(publicId);
            return result ?? false;
        }
    }
}