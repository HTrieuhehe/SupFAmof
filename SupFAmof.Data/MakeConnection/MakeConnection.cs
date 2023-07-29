using SupFAmof.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SupFAmof.Data.MakeConnection
{
    public static class MakeConnection
    {
        public static IServiceCollection ConnectToConnectionString(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SupFAmof_Stg_dbContext>(options =>
            {
                options.UseLazyLoadingProxies();
            });
            return services;
        }
    }
}
