using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventure.DB
{
    public static class DI
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(config.GetConnectionString("EventureDbPostgres")));
            return services;
        }
    }
}
