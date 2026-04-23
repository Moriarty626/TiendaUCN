using CloudinaryDotNet.Actions;

namespace TiendaUCN.Application.Services.Interfaces;

public interface IPhotoService
{
    // Para subir la imagen y obtener la URL y el PublicId
    Task<ImageUploadResult> AddPhotoAsync(IFormFile file);

    // Para borrar la imagen de la nube (necesario para el Soft Delete)
    Task<DeletionResult> DeletePhotoAsync(string publicId);
}