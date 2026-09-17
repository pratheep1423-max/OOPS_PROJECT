using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAttendanceManagementSystem.Models;
using StudentAttendanceManagementSystem.Models.ViewModels;
using StudentAttendanceManagementSystem.Services;

namespace StudentAttendanceManagementSystem.Controllers
{
    [Authorize]
    public class AttendanceController : Controller
    {
        private readonly IAttendanceService _attendanceService;
        private readonly IStudentService _studentService;

        public AttendanceController(IAttendanceService attendanceService, IStudentService studentService)
        {
            _attendanceService = attendanceService;
            _studentService = studentService;
        }

        // GET: Attendance/Mark
        public async Task<IActionResult> Mark(DateTime? attendanceDate, string? department, int? year, string? section)
        {
            var date = attendanceDate ?? DateTime.Today;

            var model = new AttendanceMarkViewModel
            {
                AttendanceDate = date,
                Department = department,
                Year = year,
                Section = section,
                Departments = await _studentService.GetDistinctDepartmentsAsync(),
                Sections = await _studentService.GetDistinctSectionsAsync()
            };

            if (!string.IsNullOrWhiteSpace(department) || year.HasValue || !string.IsNullOrWhiteSpace(section))
            {
                model.Students = await _attendanceService.GetStudentsForMarkingAsync(date, department, year, section);
            }

            return View(model);
        }

        // POST: Attendance/Mark
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Mark(AttendanceMarkViewModel model)
        {
            model.Departments = await _studentService.GetDistinctDepartmentsAsync();
            model.Sections = await _studentService.GetDistinctSectionsAsync();

            if (model.Students == null || model.Students.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "No students selected to mark attendance for.");
                return View(model);
            }

            var (success, error) = await _attendanceService.SaveAttendanceAsync(model.AttendanceDate, model.Students);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Unable to save attendance.");
                return View(model);
            }

            TempData["SuccessMessage"] = $"Attendance saved for {model.AttendanceDate:dd-MMM-yyyy}.";
            return RedirectToAction(nameof(Mark), new
            {
                attendanceDate = model.AttendanceDate,
                department = model.Department,
                year = model.Year,
                section = model.Section
            });
        }

        // GET: Attendance/Records
        public async Task<IActionResult> Records(DateTime? dateFilter, string? registerNumberSearch, string? departmentFilter, int pageNumber = 1)
        {
            var filter = new AttendanceRecordsViewModel
            {
                DateFilter = dateFilter,
                RegisterNumberSearch = registerNumberSearch,
                DepartmentFilter = departmentFilter,
                PageNumber = pageNumber
            };

            var result = await _attendanceService.GetRecordsAsync(filter);
            return View(result);
        }

        // POST: Attendance/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int attendanceId, AttendanceStatus status, DateTime? dateFilter, string? registerNumberSearch, string? departmentFilter)
        {
            var (success, error) = await _attendanceService.UpdateAttendanceStatusAsync(attendanceId, status);

            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Attendance record updated." : (error ?? "Update failed.");

            return RedirectToAction(nameof(Records), new { dateFilter, registerNumberSearch, departmentFilter });
        }
    }
}
