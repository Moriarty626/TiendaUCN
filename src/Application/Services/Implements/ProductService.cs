using Mapster;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaUCN.src.Application.DTOs.ProductDTO;
using TiendaUCN.src.Application.DTOs.ProductDTO.Admin;
using TiendaUCN.src.Application.DTOs.ProductDTO.Customer;
using TiendaUCN.src.Application.Services.Interfaces;
using TiendaUCN.src.Domain.Models;

namespace TiendaUCN.src.Application.Services.Implements
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
            var categoryEx = await _categoryRepository.ExistsNameAsync(createProductDTO.CategoryName);
            if(!categoryEx)
            {
                Log.Error($"La categoría '{createProductDTO.CategoryName}' no existe.");
                throw new Exception("La categoría especificada no existe" + createProductDTO.CategoryName);
            }
            var productEx = await _productRepository.ExistsNameAndBrandAsync(createProductDTO.Name, createProductDTO.BrandName);
            if(productEx)
            {
                Log.Error("El producto de nombre y marca: {Name} - {Brand}", createProductDTO.Name, createProductDTO.BrandName,"ya existen");
                throw new Exception("El producto de nombre y marca: {Name} - {Brand}" + createProductDTO.Name + "-" + createProductDTO.BrandName + "ya existen");
            }

            var brandEx = await _brandRepository.ExistsNameAsync(createProductDTO.BrandName);
            if(!brandEx)
            {
                Log.Error($"La marca {createProductDTO.BrandName}' no existe.");
                throw new Exception("La marca no existe" + createProductDTO.BrandName);
            }

            //Crear producto
            var product = createProductDTO.Adapt<Product>();
            var categoryId = await _categoryRepository.GetIdByNameAsync(createProductDTO.CategoryName);
            var brandId = await _brandRepository.GetIdByNameAsync(createProductDTO.BrandName);
            //Debe ser agregado a la basedata
            var created = await _productRepository.CreateProductAsync(product);
            if(!created)
            {
                Log.Error("Error al crear el producto {Name}", createProductDTO.Name);
                throw new Exception("Error al crear el producto" + createProductDTO.Name);
            }

            foreach (var image in createProductDTO.ImagesFiles)
            {
                Log.Information("Esta Imagen se asocia al producto: {@Image}", image);
                await _imageService.UploadImageAsync(image, product.Id);
            }
            return product.Id.ToString();
        }

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
            var(products, totalCount) = await _productRepository.GetFilteredAdminAsync(searchParams);
            if(totalCount == 0)
            {
                Log.Warning("No se encontraron productos para los parámetros de búsqueda para el admin. Filtros: {@SearchParams}.", searchParams);
                throw new KeyNotFoundException("No se encontraron productos para los parámetros de búsqueda para el admin.");
            }
            var totalPages = (int)Math.Ceiling((double)totalCount / searchParams.PageSize);
            var ProductPage = products.Count();

            if(searchParams.PageNumber > totalPages)
            {
                Log.Warning("El número de página {PageNumber} excede el total de páginas {TotalPages} para los parámetros de búsqueda para el admin. Filtros: {@SearchParams}.", searchParams.PageNumber, totalPages, searchParams);
                throw new KeyNotFoundException($"La pagina {searchParams.PageNumber} excede el total de páginas {totalPages}.");
            }

            return new ListedProductsForAdminDTO
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                PagesSize = searchParams.PageSize,
                ProductPage = ProductPage,
                CurrentPage = searchParams.PageNumber,
                Products = products.Adapt<List<ProductForAdminDTO>>()
            };
        }

        public async Task<ListedProductsForCustomerDTO> GetListedProductsForCustomerAsync(SearchParamsDTO searchParams)
        {
            var(products, totalCount) = await _productRepository.GetFilteredCustomerAsync(searchParams);
            if(totalCount == 0)
            {
                Log.Warning("No se encontraron productos para los parámetros de búsqueda para el admin. Filtros: {@SearchParams}.", searchParams);
                throw new KeyNotFoundException("No se encontraron productos para los parámetros de búsqueda para el admin.");
            }
            var totalPages = (int)Math.Ceiling((double)totalCount / searchParams.PageSize);
            var ProductPage = products.Count();

            if(searchParams.PageNumber > totalPages)
            {
                Log.Warning("El número de página {PageNumber} excede el total de páginas {TotalPages} para los parámetros de búsqueda para el admin. Filtros: {@SearchParams}.", searchParams.PageNumber, totalPages, searchParams);
                throw new KeyNotFoundException($"La pagina {searchParams.PageNumber} excede el total de páginas {totalPages}.");
            }

            return new ListedProductsForCustomerDTO
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                PagesSize = searchParams.PageSize,
                ProductPage = ProductPage,
                CurrentPage = searchParams.PageNumber,
                Products = products.Adapt<List<ProductForCustomerDTO>>()
            };
        }

        public async Task<ProductDetailAdminDTO> GetProductByIdForAdminAsync(int id)
        {
            var productEx = await _productRepository.ExistsIdAsync(id);
            if(!productEx)
            {
                Log.Error($"El producto con id {id} no fue encontrado.");
                throw new Exception("El producto no fue encontrado" + id);
            }
            Product product = await _productRepository.GetProductIdAdminAsync(id) 
                ?? throw new Exception("Error al obtener el producto" + id);
            if(product.Category.DeletedAt)
            {
                product.Category.Name = "La categoría ha sido eliminada";
                product.Category.Description = "La categoría ha sido eliminada";
            }
            if(product.Brand.DeletedAt)
            {
                product.Brand.Name = "La marca ha sido eliminada";
                product.Brand.Description = "La marca ha sido eliminada";
            }
            var productDetailAdminDTO = product.Adapt<ProductDetailAdminDTO>();
            return productDetailAdminDTO!;
        }

        public async Task<ProductDetailCustomerDTO> GetProductByIdForCustomerAsync(int id)
        {
            var productEx = await _productRepository.ExistsIdCustomerAsync(id);
            if(!productEx)
            {
                Log.Error($"El producto con id {id} no fue encontrado o no está activo.");
                throw new Exception("El producto no fue encontrado o no está activo" + id);
            }
            
            var product = await _productRepository.GetProductIdCustomerAsync(id)
                ?? throw new KeyNotFoundException("Producto no encontrado con ID: " + id);
            if (product.Category.DeletedAt)
            {
                product.Category.Name = "La categoría ha sido eliminada";
                product.Category.Description = "La categoría ha sido eliminada";
            }
            
            if (product.Brand.DeletedAt)
            {
                product.Brand.Name = "La marca ha sido eliminada";
                product.Brand.Description = "La marca ha sido eliminada";
            }

            var productDetailCustomerDTO = product.Adapt<ProductDetailCustomerDTO>();
            return productDetailCustomerDTO!;
        }

        public async Task<string> SwitchProductStatusAsync(int id)
        {
           var productEx = await _productRepository.ExistsIdAsync(id);
           if(!productEx)
            {
                Log.Error($"El producto con id {id} no fue encontrado.");
                throw new Exception("El producto no fue encontrado" + id);
            }
            var updated = await _productRepository.SwitchStatusAsync(id);
            
            if(!updated)
            {
                Log.Error($"Error al cambiar el estado del producto con id {id}.", id);
                throw new Exception("Error al cambiar el estado del producto" + id);
            }
            var statusU= await _productRepository.GetStatusAsync(id);
            return "Se cambio el estado a: " + statusU;
        
            
        }

        public Task UpdateProductAsync(int id, UpdateProductDTO updateProductDTO)
        {
            throw new NotImplementedException();
        }
    }
}