using Microsoft.EntityFrameworkCore;
using TiendaUCN.Infrastructure.Data.Migrations;
using TiendaUCN.src.Application.DTOs.OrderDTO;
using TiendaUCN.src.Application.Services.Interfaces;

namespace TiendaUCN.src.Application.Services.Implements
{
    public class OrderService(DataContext context) : IOrderService
    {
        public async Task<IEnumerable<OrderResponseDTO>> GetUserOrderHistoryAsync(int userId)
        {
            var orders = await context.Orders
                .Include(o => o.OrderDetails)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(o => new OrderResponseDTO
            {
                Id = o.Id,
                OrderCode = o.OrderCode,
                Total = o.Total,
                CreatedAt = o.CreatedAt,
                Details = o.OrderDetails.Select(od => new OrderDetailResponseDTO
                {
                    ProductId = od.ProductId,
                    ProductName = od.ProductName,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    Subtotal = od.Subtotal
                }).ToList()
            });
        }

        public async Task<OrderResponseDTO?> GetOrderByIdAsync(int orderId, int userId)
        {
            var o = await context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (o == null) return null;

            return new OrderResponseDTO
            {
                Id = o.Id,
                OrderCode = o.OrderCode,
                Total = o.Total,
                CreatedAt = o.CreatedAt,
                Details = o.OrderDetails.Select(od => new OrderDetailResponseDTO
                {
                    ProductId = od.ProductId,
                    ProductName = od.ProductName,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    Subtotal = od.Subtotal
                }).ToList()
            };
        }
    }
}