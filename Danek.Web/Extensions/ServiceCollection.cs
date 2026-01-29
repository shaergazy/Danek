using Danek.BLL.Services;
using Danek.BLL.Services.Contracts;
using Danek.DAL;
using Danek.DAL.Models.Users;
using Danek.DAL.Repositories;
using Danek.DAL.Repositories.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Danek.Web.Extensions
{
    public static class ServiceCollection
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterServices(services, configuration);
        }

        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<DbContext>(sp => sp.GetRequiredService<AppDbContext>());

            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            services.AddScoped(typeof(IUnitOfWork<,>), typeof(UnitOfWork<,>));
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<UserManager<User>>();
        }
    }
}
