using System.ComponentModel.DataAnnotations;

namespace StudentAttendanceManagementSystem.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Register number is required.")]
        [StringLength(50)]
        [Display(Name = "Register Number")]
        public string RegisterNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Student name is required.")]
        [StringLength(100)]
        [Display(Name = "Student Name")]
        public string StudentName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department is required.")]
        [StringLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required(ErrorMessage = "Year is required.")]
        [Range(1, 5, ErrorMessage = "Year must be between 1 and 5.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Section is required.")]
        [StringLength(10)]
        public string Section { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Enter a valid phone number.")]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        public ICollection<Attendance> AttendanceRecords { get; set; } = new List<Attendance>();
    }
}
