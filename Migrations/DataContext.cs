using Microsoft.EntityFrameworkCore;
using TiendaUCN.Domain.Models.Order;
using TiendaUCN.Domain.Models.Product;
using TiendaUCN.Domain.Models.User;
using TiendaUCN.src.Domain.JwtBlacklist;
using TiendaUCN.src.Domain.Models.Cart;
using TiendaUCN.src.Domain.Models.Product;

namespace TiendaUCN.Infrastructure.Data.Migrations
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Brand> Brands { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Image> Images { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!; 
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!; 
        public DbSet<JwtBlacklist> JwtBlacklist { get; set; } = null!;
        public DbSet<VerificationCode> VerificationCodes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Role ────────────────────────────────────────────────────────
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Name).IsRequired().HasMaxLength(50);
                entity.HasIndex(r => r.Name).IsUnique();
            });

            // ── User ────────────────────────────────────────────────────────
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Rut).IsRequired().HasMaxLength(12);
                entity.HasIndex(u => u.Rut).IsUnique();
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ── Product ─────────────────────────────────────────────────────
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(150);
                entity.Property(p => p.Price).HasPrecision(18, 2); // Precisión para moneda
                entity.Property(p => p.IsActive).HasDefaultValue(true);

                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Brand)
                    .WithMany(b => b.Products)
                    .HasForeignKey(p => p.BrandId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ── CartItem ────────────────────────────────────────────────────
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(ci => ci.Id);
                entity.Property(ci => ci.Quantity).IsRequired();
                
                entity.HasOne(ci => ci.Cart)
                    .WithMany() 
                    .HasForeignKey(ci => ci.CartId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ci => ci.Product)
                    .WithMany()
                    .HasForeignKey(ci => ci.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ── Order & OrderDetail
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.Total).HasPrecision(18, 2);
                entity.Property(o => o.OrderCode).IsRequired().HasMaxLength(20);
                entity.HasIndex(o => o.OrderCode).IsUnique();

                entity.HasOne(o => o.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Restrict); 
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(od => od.Id);
                entity.Property(od => od.UnitPrice).HasPrecision(18, 2);
                entity.Property(od => od.Subtotal).HasPrecision(18, 2);

                entity.HasOne(od => od.Order)
                    .WithMany(o => o.OrderDetails)
                    .HasForeignKey(od => od.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(od => od.Product)
                    .WithMany()
                    .HasForeignKey(od => od.ProductId)
                    .OnDelete(DeleteBehavior.Restrict); 
            });

            // ── JwtBlacklist ────────────────────────────────────────────────
            modelBuilder.Entity<JwtBlacklist>(entity =>
            {
                entity.HasKey(j => j.Id);
                entity.HasIndex(j => j.TokenId).IsUnique();
                entity.Property(j => j.InvalidatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }
    }
}