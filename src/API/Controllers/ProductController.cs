using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaUCN.Application.DTOs.ProductDTO.Admin;
using TiendaUCN.src.Application.DTOs.BaseResponse;
using TiendaUCN.src.Application.DTOs.ProductDTO;
using TiendaUCN.src.Application.DTOs.ProductDTO.Admin;
using TiendaUCN.src.Application.DTOs.ProductDTO.Customer;
using TiendaUCN.src.Application.Services.Interfaces;

namespace TiendaUCN.src.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]

    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]


        public async Task<IActionResult> CreateProduct([FromForm] ProductCreateDTO productCreateDTO)
        {
            var result = await _productService.CreateProductAsync(productCreateDTO);
            return Created($"/api/product/{result}", new GenericResponse<string>("El producto ha sido creado exitosamente", result));

        }
        [HttpPut("switch-status/{id}")]
        public async Task<IActionResult> SwitchProductStatus([FromRoute] int id)
        {
            var result = await _productService.SwitchProductStatusAsync(id);
            return Ok(new GenericResponse<string>("El estado del producto ha sido cambiado exitosamente", id.ToString()));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductByIdForCustomerAsync([FromRoute] int id)
        {
            var result = await _productService.GetProductByIdForCustomerAsync(id);
            return Ok(new GenericResponse<ProductDetailCustomerDTO>("Producto encontrado", result));
        }

        [HttpGet("admin/{id}")]
        public async Task<IActionResult> GetProductByIdForAdmin([FromRoute] int id)
        {
            var result = await _productService.GetProductByIdForAdminAsync(id);
            return Ok(new GenericResponse<ProductDetailAdminDTO>("Producto encontrado", result));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            await _productService.DeleteProductAsync(id);
            return Ok(new GenericResponse<string>("El producto ha sido eliminado", null));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ListedProductsForCustomerDTO([FromQuery] SearchParamsDTO searchParams)
        {
            var result = await _productService.GetListedProductsForCustomerAsync(searchParams);
            return Ok(new GenericResponse<ListedProductsForCustomerDTO>("Productos encontrados", result));
        }
        [HttpGet("admin")]

        public async Task<IActionResult> ListedProductsForAdminDTO([FromQuery] SearchParamsDTO searchParams)
        {
            var result = await _productService.GetListedProductsForAdminAsync(searchParams);
            return Ok(new GenericResponse<ListedProductsForAdminDTO>("Productos encontrados", result));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] UpdateProductDTO updateProductDTO)
        {
            await _productService.UpdateProductAsync(id, updateProductDTO);
            return Ok(new GenericResponse<string>("El producto ha sido actualizado exitosamente", null));
        }

    }

}