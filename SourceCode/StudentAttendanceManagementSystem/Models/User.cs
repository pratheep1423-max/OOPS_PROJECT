using System.ComponentModel.DataAnnotations;

namespace StudentAttendanceManagementSystem.Models
{
    public enum UserRole
    {
        Admin = 1,
        Faculty = 2
    }

    public class User
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }
    }
}
