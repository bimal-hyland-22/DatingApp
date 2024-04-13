using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Helpers;
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
             services.AddScoped<IUserRepository,UserRepository>();
             services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
             services.Configure<CloudnarySettings>(config.GetSection("CloudinarySettings"));
             services.AddScoped<IPhotoService,PhotoService>();
             services.AddScoped<LoggedUserActivity>();
             services.AddScoped<ILikeRepository,UserLikeRepository>();
             services.AddScoped<IMessageRepository,MessageRepository>();

             return services;
         }
    }
}