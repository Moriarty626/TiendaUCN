using Microsoft.EntityFrameworkCore;
using TiendaUCN.Domain.Models.Product;
using TiendaUCN.Infrastructure.Data.Migrations;
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
            await _context.Images.AddAsync(image);
            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<bool?> DeleteAsync(string publicId)
        {
            var image = await _context.Images.FirstOrDefaultAsync(i => i.PublicId == publicId);
            if (image == null) return false;

            image.IsActive = false; // Borrado lógico
            return await _context.SaveChangesAsync() > 0;
        }
    }
}