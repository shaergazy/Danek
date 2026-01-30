using Danek.BLL.Services;
using Danek.BLL.Services.Contracts;
using Danek.DAL;
using Danek.DAL.Repositories;
using Danek.DAL.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Danek.Web.Extensions
{
    public static class ServiceCollection
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterServices(services, configuration);
            services.ConfigMapper();
        }

        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<DbContext>(sp => sp.GetRequiredService<AppDbContext>());

            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            services.AddScoped(typeof(IUnitOfWork<,>), typeof(UnitOfWork<,>));
            services.AddTransient<IBookService, BookService>();
        }
    }
}
