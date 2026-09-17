using Microsoft.EntityFrameworkCore;
using StudentAttendanceManagementSystem.Data;
using StudentAttendanceManagementSystem.Models;
using StudentAttendanceManagementSystem.Models.ViewModels;

namespace StudentAttendanceManagementSystem.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly ApplicationDbContext _context;

        public AttendanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<StudentAttendanceRow>> GetStudentsForMarkingAsync(DateTime date, string? department, int? year, string? section)
        {
            var query = _context.Students.AsQueryable();

            if (!string.IsNullOrWhiteSpace(department))
                query = query.Where(s => s.Department == department);

            if (year.HasValue)
                query = query.Where(s => s.Year == year.Value);

            if (!string.IsNullOrWhiteSpace(section))
                query = query.Where(s => s.Section == section);

            var dateOnly = date.Date;

            var students = await query
                .OrderBy(s => s.RegisterNumber)
                .Select(s => new StudentAttendanceRow
                {
                    StudentId = s.StudentId,
                    RegisterNumber = s.RegisterNumber,
                    StudentName = s.StudentName,
                    IsPresent = true,
                    AlreadyMarked = _context.AttendanceRecords
                        .Any(a => a.StudentId == s.StudentId && a.AttendanceDate == dateOnly)
                })
                .ToListAsync();

            // Pre-fill existing status for already-marked students
            var existingRecords = await _context.AttendanceRecords
                .Where(a => a.AttendanceDate == dateOnly && students.Select(x => x.StudentId).Contains(a.StudentId))
                .ToListAsync();

            foreach (var s in students)
            {
                var existing = existingRecords.FirstOrDefault(r => r.StudentId == s.StudentId);
                if (existing != null)
                {
                    s.IsPresent = existing.Status == AttendanceStatus.Present;
                }
            }

            return students;
        }

        public async Task<(bool Success, string? Error)> SaveAttendanceAsync(DateTime date, List<StudentAttendanceRow> rows)
        {
            if (rows == null || rows.Count == 0)
            {
                return (false, "No students to save attendance for.");
            }

            var dateOnly = date.Date;
            var studentIds = rows.Select(r => r.StudentId).ToList();

            var existing = await _context.AttendanceRecords
                .Where(a => a.AttendanceDate == dateOnly && studentIds.Contains(a.StudentId))
                .ToListAsync();

            foreach (var row in rows)
            {
                var status = row.IsPresent ? AttendanceStatus.Present : AttendanceStatus.Absent;
                var existingRecord = existing.FirstOrDefault(e => e.StudentId == row.StudentId);

                if (existingRecord != null)
                {
                    // Update instead of creating a duplicate for the same date
                    existingRecord.Status = status;
                }
                else
                {
                    _context.AttendanceRecords.Add(new Attendance
                    {
                        StudentId = row.StudentId,
                        AttendanceDate = dateOnly,
                        Status = status,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException)
            {
                return (false, "A database error occurred while saving attendance. Please try again.");
            }
        }

        public async Task<AttendanceRecordsViewModel> GetRecordsAsync(AttendanceRecordsViewModel filter)
        {
            var query = _context.AttendanceRecords
                .Include(a => a.Student)
                .AsQueryable();

            if (filter.DateFilter.HasValue)
            {
                var d = filter.DateFilter.Value.Date;
                query = query.Where(a => a.AttendanceDate == d);
            }

            if (!string.IsNullOrWhiteSpace(filter.RegisterNumberSearch))
            {
                query = query.Where(a => a.Student!.RegisterNumber.Contains(filter.RegisterNumberSearch));
            }

            if (!string.IsNullOrWhiteSpace(filter.DepartmentFilter))
            {
                query = query.Where(a => a.Student!.Department == filter.DepartmentFilter);
            }

            filter.TotalCount = await query.CountAsync();

            var page = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize < 1 ? 15 : filter.PageSize;

            filter.Records = await query
                .OrderByDescending(a => a.AttendanceDate)
                .ThenBy(a => a.Student!.RegisterNumber)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AttendanceRecordRow
                {
                    AttendanceId = a.AttendanceId,
                    RegisterNumber = a.Student!.RegisterNumber,
                    StudentName = a.Student.StudentName,
                    Department = a.Student.Department,
                    AttendanceDate = a.AttendanceDate,
                    Status = a.Status
                })
                .ToListAsync();

            filter.Departments = await _context.Students
                .Select(s => s.Department).Distinct().OrderBy(d => d).ToListAsync();

            return filter;
        }

        public async Task<(bool Success, string? Error)> UpdateAttendanceStatusAsync(int attendanceId, AttendanceStatus status)
        {
            var record = await _context.AttendanceRecords.FindAsync(attendanceId);
            if (record == null)
            {
                return (false, "Attendance record not found.");
            }

            record.Status = status;
            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            var today = DateTime.Today;

            var totalStudents = await _context.Students.CountAsync();

            var todayRecords = await _context.AttendanceRecords
                .Where(a => a.AttendanceDate == today)
                .ToListAsync();

            var present = todayRecords.Count(a => a.Status == AttendanceStatus.Present);
            var absent = todayRecords.Count(a => a.Status == AttendanceStatus.Absent);
            var markedTotal = todayRecords.Count;

            double percentage = markedTotal == 0 ? 0 : Math.Round((present / (double)markedTotal) * 100, 2);

            var recent = await _context.AttendanceRecords
                .Include(a => a.Student)
                .OrderByDescending(a => a.CreatedAt)
                .Take(10)
                .ToListAsync();

            return new DashboardViewModel
            {
                TotalStudents = totalStudents,
                PresentToday = present,
                AbsentToday = absent,
                TodayAttendancePercentage = percentage,
                RecentAttendanceRecords = recent
            };
        }
    }
}
