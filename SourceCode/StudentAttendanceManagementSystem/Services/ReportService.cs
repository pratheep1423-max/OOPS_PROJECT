using Microsoft.EntityFrameworkCore;
using StudentAttendanceManagementSystem.Data;
using StudentAttendanceManagementSystem.Models;
using StudentAttendanceManagementSystem.Models.ViewModels;

namespace StudentAttendanceManagementSystem.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ReportFilterViewModel> GenerateReportAsync(ReportFilterViewModel filter)
        {
            var studentQuery = _context.Students.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Department))
                studentQuery = studentQuery.Where(s => s.Department == filter.Department);

            if (filter.Year.HasValue)
                studentQuery = studentQuery.Where(s => s.Year == filter.Year.Value);

            if (!string.IsNullOrWhiteSpace(filter.Section))
                studentQuery = studentQuery.Where(s => s.Section == filter.Section);

            if (filter.StudentId.HasValue)
                studentQuery = studentQuery.Where(s => s.StudentId == filter.StudentId.Value);

            var students = await studentQuery.ToListAsync();
            var studentIds = students.Select(s => s.StudentId).ToList();

            var attendanceQuery = _context.AttendanceRecords
                .Where(a => studentIds.Contains(a.StudentId));

            if (filter.FromDate.HasValue)
                attendanceQuery = attendanceQuery.Where(a => a.AttendanceDate >= filter.FromDate.Value.Date);

            if (filter.ToDate.HasValue)
                attendanceQuery = attendanceQuery.Where(a => a.AttendanceDate <= filter.ToDate.Value.Date);

            var attendanceRecords = await attendanceQuery.ToListAsync();

            var results = new List<StudentReportRow>();

            foreach (var student in students)
            {
                var studentRecords = attendanceRecords.Where(a => a.StudentId == student.StudentId).ToList();
                var totalWorkingDays = studentRecords.Select(a => a.AttendanceDate).Distinct().Count();
                var present = studentRecords.Count(a => a.Status == AttendanceStatus.Present);
                var absent = studentRecords.Count(a => a.Status == AttendanceStatus.Absent);

                results.Add(new StudentReportRow
                {
                    StudentId = student.StudentId,
                    RegisterNumber = student.RegisterNumber,
                    StudentName = student.StudentName,
                    Department = student.Department,
                    TotalWorkingDays = totalWorkingDays,
                    DaysPresent = present,
                    DaysAbsent = absent
                });
            }

            filter.Results = results.OrderBy(r => r.RegisterNumber).ToList();
            filter.Departments = await _context.Students.Select(s => s.Department).Distinct().OrderBy(d => d).ToListAsync();

            return filter;
        }

        public async Task<LowAttendanceViewModel> GetLowAttendanceStudentsAsync(double threshold)
        {
            var students = await _context.Students.ToListAsync();
            var allAttendance = await _context.AttendanceRecords.ToListAsync();

            var results = new List<StudentReportRow>();

            foreach (var student in students)
            {
                var studentRecords = allAttendance.Where(a => a.StudentId == student.StudentId).ToList();
                var totalWorkingDays = studentRecords.Select(a => a.AttendanceDate).Distinct().Count();

                if (totalWorkingDays == 0) continue;

                var present = studentRecords.Count(a => a.Status == AttendanceStatus.Present);
                var absent = studentRecords.Count(a => a.Status == AttendanceStatus.Absent);

                var row = new StudentReportRow
                {
                    StudentId = student.StudentId,
                    RegisterNumber = student.RegisterNumber,
                    StudentName = student.StudentName,
                    Department = student.Department,
                    TotalWorkingDays = totalWorkingDays,
                    DaysPresent = present,
                    DaysAbsent = absent
                };

                if (row.AttendancePercentage < threshold)
                {
                    results.Add(row);
                }
            }

            return new LowAttendanceViewModel
            {
                Threshold = threshold,
                Students = results.OrderBy(r => r.AttendancePercentage).ToList()
            };
        }
    }
}
