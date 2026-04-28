using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaUCN.src.Application.Services.Interfaces
{
    public interface IImageService
    {
        Task<bool> UploadImageAsync(IFormFile file, int productId);
        Task<bool> DeleteImageAsync(string publicId);
    }
}