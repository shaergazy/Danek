using Danek.BLL.Constants;
using Danek.BLL.DTOs;
using Danek.BLL.Extesions;
using Danek.DAL;
using Danek.DAL.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace Danek.Web.Extensions
{
    public static class ApplicationBuilderExtension
    {
        internal static void RegisterVirtualDir(this IApplicationBuilder app, IConfiguration configuration)
        {
            var settings = configuration.GetSection(nameof(SettingsDto.VirtualDir)).Get<SettingsDto.VirtualDir>();
            var dir = Directory.GetCurrentDirectory().Combine(settings.BaseDir);
            dir.CreateDirectoryIfNotExist();
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(dir),
                RequestPath = new PathString(settings.BaseSuffixUri)
            });
            AppConstants.RelativeFilesPath = settings.BaseDir;
            AppConstants.BaseDir = dir;
        }

        public static void InitializeDatabase(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
            var services = app.ApplicationServices.GetService<IServiceProvider>();
            DatabaseMigrator.ReseedDatabaseAsync(services).GetAwaiter().GetResult();
        }
    }
}
