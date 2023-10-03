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
            services.AddDbContext<SupFAmOf_Stg_Db_Ver_2Context>(options =>
            {
                options.UseLazyLoadingProxies();
                options.UseSqlServer(configuration.GetConnectionString("SQLServerDatabase"));
            });
            services.AddDbContext<SupFAmOf_Stg_Db_Ver_2Context>(ServiceLifetime.Transient);
            return services;
        }
    }
}
