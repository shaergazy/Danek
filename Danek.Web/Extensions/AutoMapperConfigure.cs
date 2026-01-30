using AutoMapper;
using Danek.BLL.Infrastructure;

namespace Danek.Web.Extensions
{
    public static class AutoMapperConfigure
    {
        public static void ConfigMapper(this IServiceCollection services)
        {
            services.AddSingleton(_ => new MapperConfiguration(cfg =>
            {
                cfg.AllowNullCollections = true;
                cfg.ShouldMapMethod = (m => false);
                cfg.AddProfile(new BLL.Infrastructure.AutoMapperProfile());
                cfg.AddProfile(new AutoMapperProfile());
            }).CreateMapper());
        }
    }
}
