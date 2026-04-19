using TiendaUCN.src.Domain.Models;

namespace TiendaUCN.Models
{
    public interface IUserRepository
    {
        Task<bool> ExistsByNameAsync(string name);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByRutAsync(string rut);
        Task<bool> ExistsByPhoneNumberAsync(string phoneNumber);
        Task CreateAsync(User user);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task<bool> MarkEmailAsVerifiedAsync(int id);
        Task<bool> MarkEmailAsConfirmedAsync(int id);
        Task<int> DeleteUnconfirmedUsersAsync(int daysToDeleteUnverifiedAccount);
        Task<bool> SaveVerificationCodeAsync(int userId, string verificationCode, DateTime verificationCodeExpiry);
    }
}