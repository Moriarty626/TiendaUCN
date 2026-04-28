namespace TiendaUCN.src.Application.DTOs.BrandDTO

{
    public interface IBrandService
    {
        Task<string> CreateBrandAsync(BrandCreateDTO brandDto);
        Task<string> UpdateBrandAsync(int brandId, UpdateBrandDTO brandDto);
        Task<string> DeleteBrandAsync(int brandId);
    }
}