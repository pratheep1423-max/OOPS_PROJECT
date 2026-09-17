namespace StudentAttendanceManagementSystem.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int PresentToday { get; set; }
        public int AbsentToday { get; set; }
        public double TodayAttendancePercentage { get; set; }
        public List<Attendance> RecentAttendanceRecords { get; set; } = new();
    }
}
