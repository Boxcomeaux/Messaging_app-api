using api.Data;
using api.Interfaces;
using api.Services;
using Microsoft.EntityFrameworkCore;

namespace api.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config){
            services.AddScoped<ITokenService, TokenService>();
            services.AddDbContext<DataContext>(x => {
                x.UseSqlite(config.GetConnectionString("DevConnection"));
                });
                return services;
        }
    }
}