using Microsoft.EntityFrameworkCore;
using StudentAttendanceManagementSystem.Data;
using StudentAttendanceManagementSystem.Models;

namespace StudentAttendanceManagementSystem.Services
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _context;

        public StudentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Student>> GetAllAsync(string? search = null, string? department = null, string? section = null, int? year = null)
        {
            var query = _context.Students.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(s =>
                    s.StudentName.Contains(search) ||
                    s.RegisterNumber.Contains(search) ||
                    s.Email.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(s => s.Department == department);
            }

            if (!string.IsNullOrWhiteSpace(section))
            {
                query = query.Where(s => s.Section == section);
            }

            if (year.HasValue)
            {
                query = query.Where(s => s.Year == year.Value);
            }

            return await query.OrderBy(s => s.RegisterNumber).ToListAsync();
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            return await _context.Students.FindAsync(id);
        }

        public async Task<(bool Success, string? Error)> AddAsync(Student student)
        {
            bool exists = await _context.Students
                .AnyAsync(s => s.RegisterNumber == student.RegisterNumber);

            if (exists)
            {
                return (false, "A student with this register number already exists.");
            }

            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> UpdateAsync(Student student)
        {
            bool duplicate = await _context.Students
                .AnyAsync(s => s.RegisterNumber == student.RegisterNumber && s.StudentId != student.StudentId);

            if (duplicate)
            {
                return (false, "Another student already uses this register number.");
            }

            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return false;

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<string>> GetDistinctDepartmentsAsync()
        {
            return await _context.Students
                .Select(s => s.Department)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();
        }

        public async Task<List<string>> GetDistinctSectionsAsync()
        {
            return await _context.Students
                .Select(s => s.Section)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();
        }
    }
}
