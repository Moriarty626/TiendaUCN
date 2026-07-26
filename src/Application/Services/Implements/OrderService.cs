using Microsoft.EntityFrameworkCore;
using TiendaUCN.Domain.Models.Order;
using TiendaUCN.Infrastructure.Data.Migrations;
using TiendaUCN.src.Application.DTOs.OrderDTO;
using TiendaUCN.src.Application.Services.Interfaces;

namespace TiendaUCN.src.Application.Services.Implements
{
    public class OrderService(DataContext context) : IOrderService
    {
        public async Task<OrderResponseDTO> CreateOrderAsync(int userId, CreateOrderDTO dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                throw new ArgumentException("El pedido debe tener al menos un producto.");

            var productIds = dto.Items.Select(i => i.ProductoId).ToList();
            var products = await context.Products
                .Where(p => productIds.Contains(p.Id) && p.IsActive && !p.DeletedAt)
                .ToListAsync();

            if (products.Count != productIds.Count)
                throw new KeyNotFoundException("Uno o más productos no existen o no están disponibles.");

            var order = new Order { UserId = userId };

            foreach (var item in dto.Items)
            {
                var product = products.First(p => p.Id == item.ProductoId);
                var subtotal = product.Price * item.Cantidad;
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = item.Cantidad,
                    UnitPrice = product.Price,
                    Subtotal = subtotal
                });
                order.Total += subtotal;
            }

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            return new OrderResponseDTO
            {
                Id = order.Id,
                OrderCode = order.OrderCode,
                Total = order.Total,
                CreatedAt = order.CreatedAt,
                Details = order.OrderDetails.Select(od => new OrderDetailResponseDTO
                {
                    ProductId = od.ProductId,
                    ProductName = od.ProductName,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    Subtotal = od.Subtotal
                }).ToList()
            };
        }

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