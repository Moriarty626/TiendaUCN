using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using TiendaUCN.Application.Services.Interfaces;

namespace TiendaUCN.Application.Services.Implements;

public class PhotoService : IPhotoService
{
    private readonly Cloudinary? _cloudinary;

    public PhotoService()
    {
        // Leemos las credenciales desde las variables de entorno
        var cloudName = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME");
        var apiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY");
        var apiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET");

        if (!string.IsNullOrWhiteSpace(cloudName)
            && !string.IsNullOrWhiteSpace(apiKey)
            && !string.IsNullOrWhiteSpace(apiSecret))
        {
            var acc = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(acc);
        }
    }

    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
        if (_cloudinary is null)
        {
            throw new InvalidOperationException("Cloudinary no esta configurado. Define CLOUDINARY_CLOUD_NAME, CLOUDINARY_API_KEY y CLOUDINARY_API_SECRET.");
        }

        var uploadResult = new ImageUploadResult();

        if (file.Length > 0)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                // Definimos una carpeta para organizar tus productos de limpieza
                Folder = "tienda-ucn-productos",
                Transformation = new Transformation().Height(500).Width(500).Crop("fill")
            };
            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        return uploadResult;
    }

    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        if (_cloudinary is null)
        {
            throw new InvalidOperationException("Cloudinary no esta configurado. Define CLOUDINARY_CLOUD_NAME, CLOUDINARY_API_KEY y CLOUDINARY_API_SECRET.");
        }

        var deleteParams = new DeletionParams(publicId);
        return await _cloudinary.DestroyAsync(deleteParams);
    }
}