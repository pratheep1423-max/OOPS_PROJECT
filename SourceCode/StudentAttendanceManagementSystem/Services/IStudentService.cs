using StudentAttendanceManagementSystem.Models;

namespace StudentAttendanceManagementSystem.Services
{
    public interface IStudentService
    {
        Task<List<Student>> GetAllAsync(string? search = null, string? department = null, string? section = null, int? year = null);
        Task<Student?> GetByIdAsync(int id);
        Task<(bool Success, string? Error)> AddAsync(Student student);
        Task<(bool Success, string? Error)> UpdateAsync(Student student);
        Task<bool> DeleteAsync(int id);
        Task<List<string>> GetDistinctDepartmentsAsync();
        Task<List<string>> GetDistinctSectionsAsync();
    }
}
