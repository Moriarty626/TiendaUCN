namespace TiendaUCN.Domain.Models.Product
{
    public class Image
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string PublicId { get; set; } = null!;  // ID en Cloudinary

        public int ProductId { get; set; }
        public src.Domain.Models.Product.Product Product { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }
}