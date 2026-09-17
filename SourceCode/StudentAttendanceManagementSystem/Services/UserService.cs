using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentAttendanceManagementSystem.Data;
using StudentAttendanceManagementSystem.Models;

namespace StudentAttendanceManagementSystem.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _hasher = new();

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return null;

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return result == PasswordVerificationResult.Success ? user : null;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.OrderBy(u => u.Username).ToListAsync();
        }

        public async Task<(bool Success, string? Error)> CreateUserAsync(string username, string password, UserRole role)
        {
            bool exists = await _context.Users.AnyAsync(u => u.Username == username);
            if (exists)
            {
                return (false, "A user with this username already exists.");
            }

            var user = new User { Username = username, Role = role };
            user.PasswordHash = _hasher.HashPassword(user, password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return (true, null);
        }
    }
}
