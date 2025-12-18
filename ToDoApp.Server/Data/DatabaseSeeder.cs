using Microsoft.EntityFrameworkCore;
using ToDoApp.Server.Models;

namespace ToDoApp.Server.Data;

public static class DatabaseSeeder
{
    public static async System.Threading.Tasks.Task SeedAsync(ToDoAppDbContext context)
    {
        // Check if admin user already exists
        var adminExists = await context.Users
            .AnyAsync(u => u.Username == "admin");

        if (!adminExists)
        {
            // Create admin user with default password "admin123"
            var adminUser = new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                FirstName = "Admin",
                LastName = "User",
                Role = RoleType.Admin,
                CreatedBy = 0, // System
                CreatedDate = DateTime.UtcNow
            };

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
        }
    }
}


