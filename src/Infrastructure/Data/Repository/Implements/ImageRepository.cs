using Microsoft.EntityFrameworkCore;
using TiendaUCN.src.Domain.Models;
using TiendaUCN.src.Infrastructure.Data;

namespace TiendaUCN.Infrastructure.Data.Repository.Implements
{
    public class ImageRepository : IImageRepository
    {
        private readonly DataContext _context;

        public ImageRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool?> CreateImageAsync(Image image)
        {
            // Agregamos la entidad imagen a la base de datos
            await _context.Images.AddAsync(image);

            // Guardamos los cambios y retornamos true si se guardó al menos una fila
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool?> DeleteAsync(string publicId)
        {
            // Buscamos la imagen por su PublicId de Cloudinary
            var image = await _context.Images.FirstOrDefaultAsync(i => i.PublicId == publicId);

            if (image == null) return false;

            // Eliminación física de la imagen (aquí no suele aplicarse soft delete)
            _context.Images.Remove(image);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}