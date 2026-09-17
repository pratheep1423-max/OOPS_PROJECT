using StudentAttendanceManagementSystem.Models;
using StudentAttendanceManagementSystem.Models.ViewModels;

namespace StudentAttendanceManagementSystem.Services
{
    public interface IAttendanceService
    {
        Task<List<StudentAttendanceRow>> GetStudentsForMarkingAsync(DateTime date, string? department, int? year, string? section);
        Task<(bool Success, string? Error)> SaveAttendanceAsync(DateTime date, List<StudentAttendanceRow> rows);
        Task<AttendanceRecordsViewModel> GetRecordsAsync(AttendanceRecordsViewModel filter);
        Task<(bool Success, string? Error)> UpdateAttendanceStatusAsync(int attendanceId, AttendanceStatus status);
        Task<DashboardViewModel> GetDashboardDataAsync();
    }
}
