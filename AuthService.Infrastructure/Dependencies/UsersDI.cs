using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure
{
    public static class UsersDI
    {
        public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<UsersDbContext>(options => options.UseNpgsql(config.GetConnectionString("UsersDbPostgres")));
            return services;
        }
    }
}
