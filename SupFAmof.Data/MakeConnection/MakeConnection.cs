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
            //services.AddDbContext<SupFAmOf_Stg_Db_Ver_2Context>(options =>
            //services.AddDbContext<SupFAmOf_Stg_DbContext>(options =>
            services.AddDbContext<SupFAmOf_Prod_DbContext>(options =>
            {
                options.UseLazyLoadingProxies();
                options.UseSqlServer(configuration.GetConnectionString("SQLServerDatabase"));
            });
            //services.AddDbContext<SupFAmOf_Stg_Db_Ver_2Context>(ServiceLifetime.Transient);
            //services.AddDbContext<SupFAmOf_Stg_DbContext>(ServiceLifetime.Scoped);

            //services.AddDbContext<SupFAmOf_Prod_DbContext>(ServiceLifetime.Scoped);
            services.AddTransient<SupFAmOf_Prod_DbContext>();
            return services;
        }
    }
}
