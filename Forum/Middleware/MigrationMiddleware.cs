using Microsoft.EntityFrameworkCore;
using NETForum.Data;

namespace NETForum.Middleware
{
    public class MigrationMiddleware(RequestDelegate next, ILogger<MigrationMiddleware> logger)
    {
        private static bool _migrationsApplied = false;
        private static readonly object Lock = new();

        public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
        {
            lock (Lock)
            {
                if (!_migrationsApplied)
                {
                    try
                    {
                        dbContext.Database.Migrate();
                        logger.LogInformation("Database migrations applied successfully");
                        _migrationsApplied = true;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to apply database migrations");
                        throw;
                    }
                }
            }
            await next(context);
        }
    }
}
