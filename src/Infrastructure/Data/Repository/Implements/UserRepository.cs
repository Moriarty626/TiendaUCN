using Microsoft.EntityFrameworkCore;
using Serilog;
using TiendaUCN.src.Domain.Models;

namespace TiendaUCN.src.Infrastructure.Data.Repository.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        // Inyección de dependencias del DataContext
        public UserRepository(DataContext context)
        {
            _context = context;
        }

        // Implementación de ExistsByNameAsync
        public async Task CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        // Implementación de ExistsByNameAsync
        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Users.AnyAsync(u => u.Name == name);
        }


        // Implementación de ExistsByEmailAsync
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }


        // Implementación de ExistsByRutAsync
        public async Task<bool> ExistsByRutAsync(string rut)
        {
            return await _context.Users.AnyAsync(u => u.Rut == rut);
        }
        // Implementación de ExistsByPhoneNumberAsync
        public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber);
        }

        // Implementación de GetByEmailAsync
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        // Implementación de MarkEmailAsVerifiedAsync
        public async Task<bool> MarkEmailAsVerifiedAsync(int id)
        {
            var result = await _context.Users.Where(u => u.Id == id).ExecuteUpdateAsync(u => u.SetProperty(x => x.EmailConfirmed, true));
            return result > 0;
        }

        // Implementación de DeleteUnconfirmedUsersAsync
        public async Task<int> DeleteUnconfirmedUsersAsync(int daysToDeleteUnverifiedAccount)
        {
            var now = DateTime.UtcNow;
            await _context.VerificationCodes
                .Where(vc =>
                    _context.Users
                        .Any(u =>
                            u.Id == vc.UserId &&
                            u.EmailConfirmed == false &&
                            u.IsDeleted == false &&
                            u.CreatedAt.AddDays(daysToDeleteUnverifiedAccount) <= now))
                .ExecuteDeleteAsync();
            var result = await _context.Users
                .Where(x =>
                    x.EmailConfirmed == false &&
                    x.IsDeleted == false &&
                    x.CreatedAt.AddDays(daysToDeleteUnverifiedAccount) <= now)
                .ExecuteUpdateAsync(u => u.SetProperty(x => x.IsDeleted, true));

            return result;
        }

        public async Task<bool> SaveVerificationCodeAsync(int userId, string verificationCode, DateTime VerificationCodeExpiry)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }
            user.VerificationCode = verificationCode;
            user.VerificationCodeExpiry = VerificationCodeExpiry;
            await _context.SaveChangesAsync();
            return true;
        }

        public Task<bool> MarkEmailAsConfirmedAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}