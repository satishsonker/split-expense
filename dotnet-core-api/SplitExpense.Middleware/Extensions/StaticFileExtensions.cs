using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace SplitExpense.Middleware.Extensions
{
    public static class StaticFileExtensions
    {
        public static IApplicationBuilder UseStaticFileConfiguration(this IApplicationBuilder app, IConfiguration configuration)
        {
            var uploadPath = configuration.GetValue<string>("FileUpload:StorageSettings:BasePath") ?? "uploads";
            var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), uploadPath);
            
            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            // For wwwroot folder
            app.UseStaticFiles();

            // Configure custom upload directory
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(uploadDirectory),
                RequestPath = $"/{uploadPath}"
            });

            return app;
        }
    }
} 