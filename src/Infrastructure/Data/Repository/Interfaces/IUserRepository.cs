namespace TiendaUCN.src.Domain.Models
{
    public interface IUserRepository
    {
        Task<bool> ExistsByNameAsync(string name);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByRutAsync(string rut);
        Task<bool> ExistsByPhoneNumberAsync(string phoneNumber);
        Task CreateAsync(User user);
        Task<User?> GetByEmailAsync(string email);
        Task<bool> MarkEmailAsVerifiedAsync(int id);
        Task<int> DeleteUnconfirmedUsersAsync(int daysToDeleteUnverifiedAccount);
    }
}