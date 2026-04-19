using Microsoft.EntityFrameworkCore;
using Serilog;
using TiendaUCN.src.Domain.Models;
using TiendaUCN.src.Infrastructure.Data;

namespace TiendaUCN.src.Infrastructure.Data.Repository.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Users.AnyAsync(u => u.Name == name);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> ExistsByRutAsync(string rut)
        {
            return await _context.Users.AnyAsync(u => u.Rut == rut);
        }

        public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> MarkEmailAsVerifiedAsync(int id)
        {
            var result = await _context.Users
                .Where(u => u.Id == id)
                .ExecuteUpdateAsync(u => u.SetProperty(x => x.EmailConfirmed, true));
            return result > 0;
        }

        public async Task<bool> MarkEmailAsConfirmedAsync(int id)
        {
            var result = await _context.Users
                .Where(u => u.Id == id)
                .ExecuteUpdateAsync(u => u
                    .SetProperty(x => x.EmailConfirmed, true)
                    .SetProperty(x => x.VerificationCode, (string?)null)
                    .SetProperty(x => x.VerificationCodeExpiry, (DateTime?)null));
            return result > 0;
        }

        public async Task<int> DeleteUnconfirmedUsersAsync(int daysToDeleteUnverifiedAccount)
        {
            var now = DateTime.UtcNow;
            await _context.VerificationCodes
                .Where(vc =>
                    _context.Users.Any(u =>
                        u.Id == vc.UserId &&
                        !u.EmailConfirmed &&
                        !u.IsDeleted &&
                        u.CreatedAt.AddDays(daysToDeleteUnverifiedAccount) <= now))
                .ExecuteDeleteAsync();

            return await _context.Users
                .Where(u =>
                    !u.EmailConfirmed &&
                    !u.IsDeleted &&
                    u.CreatedAt.AddDays(daysToDeleteUnverifiedAccount) <= now)
                .ExecuteUpdateAsync(u => u.SetProperty(x => x.IsDeleted, true));
        }

        public async Task<bool> SaveVerificationCodeAsync(int userId, string verificationCode, DateTime verificationCodeExpiry)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.VerificationCode = verificationCode;
            user.VerificationCodeExpiry = verificationCodeExpiry;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}