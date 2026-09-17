using System.ComponentModel.DataAnnotations;

namespace StudentAttendanceManagementSystem.Models.ViewModels
{
    public class AttendanceMarkViewModel
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime AttendanceDate { get; set; } = DateTime.Today;

        public string? Department { get; set; }
        public int? Year { get; set; }
        public string? Section { get; set; }

        public List<StudentAttendanceRow> Students { get; set; } = new();

        public List<string> Departments { get; set; } = new();
        public List<string> Sections { get; set; } = new();
    }

    public class StudentAttendanceRow
    {
        public int StudentId { get; set; }
        public string RegisterNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public bool IsPresent { get; set; } = true;
        public bool AlreadyMarked { get; set; }
    }
}
