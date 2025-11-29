using Microsoft.EntityFrameworkCore;

namespace PhoneValidate.Extensions
{
    public static class DatabaseMigrationExtensions
    {
        public static async Task ApplyDatabaseMigrationsAsync<TContext>(this IApplicationBuilder app)
            where TContext : DbContext
        {
            using var scope = app.ApplicationServices.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();

            try
            {
                logger.LogInformation("Checking database for {DbContext}...", typeof(TContext).Name);
                var canConnect = await dbContext.Database.CanConnectAsync();
                if (canConnect)
                {
                    logger.LogInformation("Database exists. Checking for pending migrations...");

                    var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
                    if (pendingMigrations.Any())
                    {
                        logger.LogInformation("Applying {Count} pending migration(s)...", pendingMigrations.Count());
                        await dbContext.Database.MigrateAsync();
                        logger.LogInformation("Migrations applied successfully.");
                    }
                    else
                    {
                        logger.LogInformation("Database is up to date. No migrations needed.");
                    }
                }
                else
                {
                    await dbContext.Database.MigrateAsync();
                    logger.LogInformation("Database created and migrations applied successfully.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while applying database migrations for {DbContext}: {Message}",
                    typeof(TContext).Name, ex.Message);
            }
        }
    }
}