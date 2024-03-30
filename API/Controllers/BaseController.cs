using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ServiceFilter(typeof(LoggedUserActivity))]
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        
    }
}