using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaUCN.src.Domain.Models
{
    public interface IImageRepository
    {
        Task<bool?> CreateImageAsync(Image image);
        Task<bool?> DeleteImageAsync(string publicId);
    }  
}