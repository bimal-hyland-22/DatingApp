using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;


namespace API.Extensions
{
    public static class ApplicationServiceExtension
    {
         public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
         {
             services.AddDbContext<DataContext>(opt=>{
              opt.UseSqlite(config.GetConnectionString("DefaultConnectionString"));
             });
             services.AddScoped<ITokenService,TokenService> ();

             return services;
         }
    }
}