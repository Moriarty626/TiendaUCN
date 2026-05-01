using TiendaUCN.src.Domain.Models.Cart;
namespace TiendaUCN.Domain.Models.User;

public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public bool EmailConfirmed { get; set; } = false;
    public string? VerificationCode { get; set; }
    public DateTime? VerificationCodeExpiry { get; set; }
    public required string Rut { get; set; }
    public required string PhoneNumber { get; set; }
    public required DateTime DateOfBirth { get; set; }
    public required string Gender { get; set; }
    public required string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int RoleId { get; set; } // Establece la relación con Role (Un rol puede tener muchos usuarios)
    public Role Role { get; set; } = null!;

    public bool IsDeleted { get; set; } = false;
    public bool IsVerified { get; set; } = false;
    // Relaciones
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public IEnumerable<Order.Order>? Orders { get; set; } = new List<Order.Order>();
}