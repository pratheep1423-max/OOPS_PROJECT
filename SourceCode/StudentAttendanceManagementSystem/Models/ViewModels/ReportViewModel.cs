namespace StudentAttendanceManagementSystem.Models.ViewModels
{
    public class ReportFilterViewModel
    {
        public string? Department { get; set; }
        public int? Year { get; set; }
        public string? Section { get; set; }
        public int? StudentId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public List<string> Departments { get; set; } = new();
        public List<StudentReportRow> Results { get; set; } = new();
    }

    public class StudentReportRow
    {
        public int StudentId { get; set; }
        public string RegisterNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public int TotalWorkingDays { get; set; }
        public int DaysPresent { get; set; }
        public int DaysAbsent { get; set; }
        public double AttendancePercentage =>
            TotalWorkingDays == 0 ? 0 : Math.Round((DaysPresent / (double)TotalWorkingDays) * 100, 2);
    }

    public class LowAttendanceViewModel
    {
        public double Threshold { get; set; } = 75;
        public List<StudentReportRow> Students { get; set; } = new();
    }
}
