using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
   public class LoggedUserActivity  : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resContext= await next();
        if (context.HttpContext.User.Identity.IsAuthenticated)
        {
            var userId = resContext.HttpContext.User.GetUserId();
            var repo=resContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
            var user=await repo.GetUserByIdAsync(int.Parse(userId));
            if (user != null)
            {
                user.LastActive = DateTime.UtcNow;
                await repo.SaveAllAsync();
            }
        }
    }
}
}