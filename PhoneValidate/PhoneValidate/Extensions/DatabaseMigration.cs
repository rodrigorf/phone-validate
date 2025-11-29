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
                logger.LogInformation("Applying database migrations for {DbContext}...", typeof(TContext).Name);
                await dbContext.Database.MigrateAsync();

                logger.LogInformation("Database migrations completed successfully for {DbContext}.", typeof(TContext).Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while applying database migrations for {DbContext}: {Message}",
                    typeof(TContext).Name, ex.Message);

            }
        }
    }
}