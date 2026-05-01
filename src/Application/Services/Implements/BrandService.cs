using TiendaUCN.src.Application.DTOs.BrandDTO;
using TiendaUCN.src.Domain.Models;

namespace TiendaUCN.Application.Services.Implements
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        public BrandService(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public Task<string> CreateBrandAsync(BrandCreateDTO brandDto)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteBrandAsync(int brandId)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateBrandAsync(int brandId, UpdateBrandDTO brandDto)
        {
            throw new NotImplementedException();
        }
    }
}