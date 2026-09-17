using Microsoft.EntityFrameworkCore;
using StudentAttendanceManagementSystem.Models;

namespace StudentAttendanceManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Attendance> AttendanceRecords { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Student configuration
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(s => s.StudentId);
                entity.HasIndex(s => s.RegisterNumber).IsUnique();
                entity.Property(s => s.RegisterNumber).IsRequired().HasMaxLength(50);
                entity.Property(s => s.StudentName).IsRequired().HasMaxLength(100);
                entity.Property(s => s.Department).IsRequired().HasMaxLength(100);
                entity.Property(s => s.Section).IsRequired().HasMaxLength(10);
                entity.Property(s => s.Email).IsRequired().HasMaxLength(150);
                entity.Property(s => s.PhoneNumber).IsRequired().HasMaxLength(20);
            });

            // Attendance configuration
            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.HasKey(a => a.AttendanceId);

                entity.HasOne(a => a.Student)
                      .WithMany(s => s.AttendanceRecords)
                      .HasForeignKey(a => a.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Prevent duplicate attendance for same student on same date
                entity.HasIndex(a => new { a.StudentId, a.AttendanceDate }).IsUnique();

                entity.Property(a => a.Status).IsRequired();
                entity.Property(a => a.AttendanceDate).IsRequired();
            });

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.HasIndex(u => u.Username).IsUnique();
                entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
                entity.Property(u => u.PasswordHash).IsRequired();
            });
        }
    }
}
