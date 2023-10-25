using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    
    public class ErrorController : BaseController
    {
        private readonly DataContext _context;
         public ErrorController(DataContext dataContext)
         {
            _context = dataContext;
            
         }
         [Authorize]
         [HttpGet("auth")]
         public ActionResult<string> GetSecrets(){
            return "Some Secret";
         }

         [HttpGet("not-found")]
         public ActionResult<AppUser> GetNotFound(){
            var user=_context.Users.Find(-1);
            if(user==null)
             return NotFound();
            return user; 
         }
         [HttpGet("server-error")]
         public ActionResult<string> GetServerError(){
             var user=_context.Users.Find(-1);
            var thing=user.ToString();
            return thing;
         }
         [HttpGet("bad-request")]
         public ActionResult<string> GetBadRequest(){
            return  BadRequest("This is not a good request");
         }

         
         
    }
}