using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaUCN.src.Domain.Models;

namespace TiendaUCN.src.Infrastructure.Data.Repository.Implements
{
    public class ImageRepository : IImageRepository
    {
        public Task<bool?> CreateImageAsync(Image image)
        {
            throw new NotImplementedException();
        }

        public Task<bool?> DeleteAsync(string publicId)
        {
            throw new NotImplementedException();
        }
    }
}