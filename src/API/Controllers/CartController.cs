using Microsoft.AspNetCore.Mvc;
using TiendaUCN.src.Application.Services.Interfaces;

namespace TiendaUCN.src.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController(ICartService cartService) : ControllerBase
    {
        // POST: api/Cart/checkout/1
        [HttpPost("checkout/{userId}")]
        public async Task<IActionResult> Checkout(int userId)
        {
            try
            {
                // Llamamos a tu servicio que tiene toda la lógica de validación y stock
                var result = await cartService.CheckoutAsync(userId);

                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                // Si el carrito está vacío o no hay stock, devolvemos un 400 con el mensaje de error
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}