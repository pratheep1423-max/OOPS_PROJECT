using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAttendanceManagementSystem.Services;

namespace StudentAttendanceManagementSystem.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IAttendanceService _attendanceService;

        public DashboardController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _attendanceService.GetDashboardDataAsync();
            return View(data);
        }
    }
}
