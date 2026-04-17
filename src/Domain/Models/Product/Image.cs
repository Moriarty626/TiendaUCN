namespace TiendaUCN.src.Domain.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string PublicId { get; set; } = null!;  // ID en Cloudinary

        // Relación con Product
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}