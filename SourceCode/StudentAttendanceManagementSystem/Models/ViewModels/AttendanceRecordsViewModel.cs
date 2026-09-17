namespace StudentAttendanceManagementSystem.Models.ViewModels
{
    public class AttendanceRecordsViewModel
    {
        public DateTime? DateFilter { get; set; }
        public string? RegisterNumberSearch { get; set; }
        public string? DepartmentFilter { get; set; }

        public List<string> Departments { get; set; } = new();
        public List<AttendanceRecordRow> Records { get; set; } = new();

        // Paging
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }

    public class AttendanceRecordRow
    {
        public int AttendanceId { get; set; }
        public string RegisterNumber { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime AttendanceDate { get; set; }
        public AttendanceStatus Status { get; set; }
    }
}
