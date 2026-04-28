using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaUCN.src.Application.Services.Interfaces;

namespace TiendaUCN.src.Application.Services.Implements
{
    public class ImageService : IImageService
    {
        public  Task<bool> DeleteImageAsync(string publicId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UploadImageAsync(IFormFile file, int productId)
        {
            throw new NotImplementedException();
        }
    }
}