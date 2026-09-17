using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StudentAttendanceManagementSystem.Models.ViewModels;
using StudentAttendanceManagementSystem.Services;

namespace StudentAttendanceManagementSystem.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IConfiguration _configuration;

        public ReportsController(IReportService reportService, IConfiguration configuration)
        {
            _reportService = reportService;
            _configuration = configuration;
        }

        // GET: Reports
        public async Task<IActionResult> Index(string? department, int? year, string? section, int? studentId, DateTime? fromDate, DateTime? toDate)
        {
            var filter = new ReportFilterViewModel
            {
                Department = department,
                Year = year,
                Section = section,
                StudentId = studentId,
                FromDate = fromDate,
                ToDate = toDate
            };

            var result = await _reportService.GenerateReportAsync(filter);
            return View(result);
        }

        // GET: Reports/LowAttendance
        public async Task<IActionResult> LowAttendance(double? threshold)
        {
            var configuredThreshold = _configuration.GetValue<double?>("AttendanceSettings:LowAttendanceThreshold") ?? 75;
            var effectiveThreshold = threshold ?? configuredThreshold;

            var result = await _reportService.GetLowAttendanceStudentsAsync(effectiveThreshold);
            return View(result);
        }
    }
}
