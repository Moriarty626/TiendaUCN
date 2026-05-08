using TiendaUCN.src.Application.DTOs.OrderDTO;

namespace TiendaUCN.src.Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponseDTO>> GetUserOrderHistoryAsync(int userId);
        Task<OrderResponseDTO?> GetOrderByIdAsync(int orderId, int userId);
    }
}