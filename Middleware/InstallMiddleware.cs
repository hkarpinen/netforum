using NETForum.Services;

namespace NETForum.Middleware
{
    public class InstallMiddleware(RequestDelegate next, ILogger<InstallMiddleware> logger)
    {
        private readonly ILogger<InstallMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context, IInstallService installService)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";

            // Handle install
            if (path.StartsWith("/install") ||
                path.StartsWith("/css") ||
                path.StartsWith("/js") ||
                path.StartsWith("/lib") ||
                path.StartsWith("/images") ||
                path.StartsWith("/favicon"))
            {
                await next(context);
                return;
            }

            var isInstallComplete = await installService.IsInstallCompleteAsync();
            if(!isInstallComplete)
            {
                context.Response.Redirect("/Install/Index");
            }

            await next(context);
        }
    }
}
