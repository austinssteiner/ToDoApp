using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ToDoApp.Server.Data;

/// <summary>
/// Centralizes database migrations and seeding so Program.cs stays lean and reusable.
/// </summary>
public static class DatabaseInitializationExtensions
{
    /// <summary>
    /// Applies pending EF Core migrations and seeds initial data in a single bootstrapping step.
    /// </summary>
    /// <param name="app">Application builder</param>
    public static async Task ApplyMigrationsAndSeedAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ToDoAppDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully.");

            await DatabaseSeeder.SeedAsync(context);
            logger.LogInformation("Database seeding completed.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to apply migrations or seed the database.");
            throw;
        }
    }
}
