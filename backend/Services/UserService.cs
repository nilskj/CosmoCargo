using CosmoCargo.Data;
using CosmoCargo.Model;
using Microsoft.EntityFrameworkCore;

namespace CosmoCargo.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> ValidateUserCredentialsAsync(string email, string password)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null)
                return false;

            return VerifyPassword(password, user.PasswordHash);
        }

        private bool VerifyPassword(string password, string hash)
        {
            return Utils.Crypto.HashPassword(password) == hash;
        }
    }
}
