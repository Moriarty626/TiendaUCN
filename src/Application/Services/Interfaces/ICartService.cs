using TiendaUCN.Domain.Models.Cart;
using TiendaUCN.src.Domain.Models.Cart;
namespace TiendaUCN.src.Application.Services.Interfaces
{
    public interface ICartService
    {
        Task<Cart> GetCartByUserIdAsync(int userId);
        Task<bool> AddItemToCartAsync(int userId, int productId, int quantity);
        Task<bool> RemoveItemFromCartAsync(int userId, int productId);
        Task<string> CheckoutAsync(int userId); // Aquí ocurre la transacción atómica
    }
}