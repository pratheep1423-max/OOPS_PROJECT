using Microsoft.AspNetCore.Identity;
using StudentAttendanceManagementSystem.Models;

namespace StudentAttendanceManagementSystem.Data
{
    /// <summary>
    /// Seeds the database with an initial Admin user and a Faculty user
    /// so the application can be logged into immediately after first run.
    /// Default credentials (CHANGE AFTER FIRST LOGIN):
    ///   admin / Admin@123
    ///   faculty / Faculty@123
    /// </summary>
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            // Assumes context.Database.Migrate() has already been called
            // (see Program.cs) so the schema exists before seeding data.
            if (!context.Users.Any())
            {
                var hasher = new PasswordHasher<User>();

                var admin = new User { Username = "admin", Role = UserRole.Admin };
                admin.PasswordHash = hasher.HashPassword(admin, "Admin@123");

                var faculty = new User { Username = "faculty", Role = UserRole.Faculty };
                faculty.PasswordHash = hasher.HashPassword(faculty, "Faculty@123");

                context.Users.AddRange(admin, faculty);
                context.SaveChanges();
            }
        }
    }
}
