namespace TiendaUCN.Models
{
    public class Role
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Relaciones
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}