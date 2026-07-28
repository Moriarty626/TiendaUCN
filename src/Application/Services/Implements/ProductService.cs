using Mapster;
using Serilog;
using TiendaUCN.Application.DTOs.ProductDTO.Admin;
using TiendaUCN.src.Application.DTOs.ProductDTO;
using TiendaUCN.src.Application.DTOs.ProductDTO.Admin;
using TiendaUCN.src.Application.DTOs.ProductDTO.Customer;
using TiendaUCN.src.Application.Services.Interfaces;
using TiendaUCN.src.Domain.Models;
using TiendaUCN.src.Domain.Models.Product;

namespace TiendaUCN.Application.Services.Implements
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IImageService _imageService;
        private readonly IConfiguration _configuration;
        private readonly IBrandRepository _brandRepository;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IImageService imageService, IConfiguration configuration, IBrandRepository brandRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _imageService = imageService;
            _configuration = configuration;
            _brandRepository = brandRepository;
        }

        public async Task<string> CreateProductAsync(ProductCreateDTO createProductDTO)
        {
            // 1. Validaciones de existencia y auto-creación de categoría/marca si no existen
            var categoryEx = await _categoryRepository.ExistsNameAsync(createProductDTO.CategoryName);
            if (!categoryEx)
            {
                var newCategory = new TiendaUCN.Domain.Models.Product.Category
                {
                    Name = createProductDTO.CategoryName,
                    Description = createProductDTO.CategoryName,
                    IsActive = true,
                    DeletedAt = false
                };
                await _categoryRepository.CreateCategoryAsync(newCategory);
            }

            var brandEx = await _brandRepository.ExistsNameAsync(createProductDTO.BrandName);
            if (!brandEx)
            {
                var newBrand = new TiendaUCN.Domain.Models.Product.Brand
                {
                    Name = createProductDTO.BrandName,
                    Description = createProductDTO.BrandName,
                    IsActive = true,
                    DeletedAt = false
                };
                await _brandRepository.CreateBrandAsync(newBrand);
            }

            var productEx = await _productRepository.ExistsNameAndBrandAsync(createProductDTO.Name, createProductDTO.BrandName);
            if (productEx)
            {
                Log.Error("El producto de nombre y marca: {Name} - {Brand} ya existe", createProductDTO.Name, createProductDTO.BrandName);
                throw new InvalidOperationException($"El producto '{createProductDTO.Name}' de la marca '{createProductDTO.BrandName}' ya existe.");
            }

            // 2. Mapeo y asignación de IDs
            var product = createProductDTO.Adapt<Product>();

            var categoryId = await _categoryRepository.GetIdByNameAsync(createProductDTO.CategoryName);
            var brandId = await _brandRepository.GetIdByNameAsync(createProductDTO.BrandName);

            product.CategoryId = categoryId;
            product.BrandId = brandId;

            // 3. Persistencia en base de datos
            var created = await _productRepository.CreateProductAsync(product);
            if (!created)
            {
                Log.Error("Error al crear el producto {Name} en la base de datos", createProductDTO.Name);
                throw new InvalidOperationException("Error al crear el producto: " + createProductDTO.Name);
            }

            // 4. Subida de imágenes a Cloudinary si fueron adjuntadas
            if (createProductDTO.ImagesFiles != null && createProductDTO.ImagesFiles.Any())
            {
                foreach (var image in createProductDTO.ImagesFiles)
                {
                    Log.Information("Subiendo imagen para el producto: {ProductName}", product.Name);
                    await _imageService.UploadImageAsync(image, product.Id);
                }
            }

            return product.Id.ToString();
        }

        // --- Resto de los métodos del servicio (sin cambios necesarios por ahora) ---

        public async Task DeleteProductAsync(int id)
        {
            var productEx = await _productRepository.ExistsIdAsync(id);
            if (!productEx)
            {
                Log.Error("Producto no encontrado con ID: {ProductId}", id);
                throw new KeyNotFoundException("Producto no encontrado con ID: " + id);
            }

            var isDeleted = await _productRepository.DeleteAsync(id);
            if (!isDeleted)
            {
                Log.Error("Error al eliminar el producto con ID: {ProductId}", id);
                throw new Exception("Error al eliminar el producto con ID: " + id);
            }
        }

        public async Task<ListedProductsForAdminDTO> GetListedProductsForAdminAsync(SearchParamsDTO searchParams)
        {
            var (products, totalCount) = await _productRepository.GetFilteredAdminAsync(searchParams);
            if (totalCount == 0)
            {
                Log.Warning("No se encontraron productos para los parámetros de búsqueda para el admin.");
                throw new KeyNotFoundException("No se encontraron productos.");
            }
            var totalPages = (int)Math.Ceiling((double)totalCount / searchParams.PageSize);

            return new ListedProductsForAdminDTO
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                PagesSize = searchParams.PageSize,
                ProductPage = products.Count(),
                CurrentPage = searchParams.PageNumber,
                Products = products.Adapt<List<ProductForAdminDTO>>()
            };
        }

        public async Task<ListedProductsForCustomerDTO> GetListedProductsForCustomerAsync(SearchParamsDTO searchParams)
        {
            var (products, totalCount) = await _productRepository.GetFilteredCustomerAsync(searchParams);
            var totalPages = totalCount == 0 ? 1 : (int)Math.Ceiling((double)totalCount / searchParams.PageSize);

            return new ListedProductsForCustomerDTO
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                PagesSize = searchParams.PageSize,
                ProductPage = products.Count(),
                CurrentPage = searchParams.PageNumber,
                Products = products.Adapt<List<ProductForCustomerDTO>>()
            };
        }

        public async Task<ProductDetailAdminDTO> GetProductByIdForAdminAsync(int id)
        {
            var product = await _productRepository.GetProductIdAdminAsync(id)
                ?? throw new KeyNotFoundException("Producto no encontrado: " + id);

            return product.Adapt<ProductDetailAdminDTO>();
        }

        public async Task<ProductDetailCustomerDTO> GetProductByIdForCustomerAsync(int id)
        {
            var product = await _productRepository.GetProductIdCustomerAsync(id)
                ?? throw new KeyNotFoundException("Producto no encontrado: " + id);

            return product.Adapt<ProductDetailCustomerDTO>();
        }

        public async Task<string> SwitchProductStatusAsync(int id)
        {
            var updated = await _productRepository.SwitchStatusAsync(id);
            if (!updated) throw new Exception("Error al cambiar el estado del producto");

            var status = await _productRepository.GetStatusAsync(id);
            return "Se cambio el estado a: " + status;
        }

        public Task UpdateProductAsync(int id, UpdateProductDTO updateProductDTO)
        {
            // Este método está pendiente por implementar según tu código original
            throw new NotImplementedException();
        }
    }
}