using Microsoft.AspNetCore.Mvc;
using TiendaUCN.src.Application.Services.Interfaces;

namespace TiendaUCN.src.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController(ICartService cartService) : ControllerBase
    {
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(int userId)
        {
            var cart = await cartService.GetCartByUserIdAsync(userId);
            return Ok(cart);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(int userId, int productId, int quantity)
        {
            var result = await cartService.AddItemToCartAsync(userId, productId, quantity);
            return Ok(new { success = result });
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromCart(int userId, int productId)
        {
            var result = await cartService.RemoveItemFromCartAsync(userId, productId);
            return Ok(new { success = result });
        }

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