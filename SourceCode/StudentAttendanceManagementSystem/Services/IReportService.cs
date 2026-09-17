using StudentAttendanceManagementSystem.Models.ViewModels;

namespace StudentAttendanceManagementSystem.Services
{
    public interface IReportService
    {
        Task<ReportFilterViewModel> GenerateReportAsync(ReportFilterViewModel filter);
        Task<LowAttendanceViewModel> GetLowAttendanceStudentsAsync(double threshold);
    }
}
