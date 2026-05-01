using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaUCN.Domain.Models.Product;

namespace TiendaUCN.src.Domain.Models
{
    public interface IImageRepository
    {
        Task<bool?> CreateImageAsync(Image image);
        Task<bool?> DeleteAsync(string publicId);
    }
}