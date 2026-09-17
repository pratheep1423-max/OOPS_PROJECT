using StudentAttendanceManagementSystem.Models;

namespace StudentAttendanceManagementSystem.Services
{
    public interface IUserService
    {
        Task<User?> ValidateUserAsync(string username, string password);
        Task<List<User>> GetAllAsync();
        Task<(bool Success, string? Error)> CreateUserAsync(string username, string password, UserRole role);
    }
}
