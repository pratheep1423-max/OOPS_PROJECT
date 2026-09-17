using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAttendanceManagementSystem.Models;
using StudentAttendanceManagementSystem.Services;

namespace StudentAttendanceManagementSystem.Controllers
{
    [Authorize]
    public class StudentsController : Controller
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // GET: Students
        public async Task<IActionResult> Index(string? search, string? department, string? section, int? year)
        {
            var students = await _studentService.GetAllAsync(search, department, section, year);

            ViewBag.Departments = await _studentService.GetDistinctDepartmentsAsync();
            ViewBag.Sections = await _studentService.GetDistinctSectionsAsync();
            ViewBag.Search = search;
            ViewBag.Department = department;
            ViewBag.Section = section;
            ViewBag.Year = year;

            return View(students);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null) return NotFound();
            return View(student);
        }

        // GET: Students/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new Student());
        }

        // POST: Students/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if (!ModelState.IsValid)
            {
                return View(student);
            }

            var (success, error) = await _studentService.AddAsync(student);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Unable to add student.");
                return View(student);
            }

            TempData["SuccessMessage"] = "Student added successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Students/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null) return NotFound();
            return View(student);
        }

        // POST: Students/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.StudentId) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(student);
            }

            var (success, error) = await _studentService.UpdateAsync(student);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Unable to update student.");
                return View(student);
            }

            TempData["SuccessMessage"] = "Student updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Students/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null) return NotFound();
            return View(student);
        }

        // POST: Students/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _studentService.DeleteAsync(id);

            TempData[deleted ? "SuccessMessage" : "ErrorMessage"] =
                deleted ? "Student deleted successfully." : "Student could not be found.";

            return RedirectToAction(nameof(Index));
        }
    }
}
