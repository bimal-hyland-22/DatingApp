using API;
using API.Controllers;
using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Namespace;
[Authorize]
public class UsersController : BaseController
{
    private readonly DataContext _context;

    public UsersController(DataContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    [HttpGet]
    public async  Task<ActionResult<IEnumerable<AppUser>>> GetUsers(){
        return await _context.Users.ToListAsync();
    }

    
    [HttpGet("{id}")]
    public  async Task<ActionResult<AppUser>> GetUser(int id){
        return  await _context.Users.FindAsync(id);
    }

}
