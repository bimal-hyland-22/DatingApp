using System.Security.Claims;
using API;
using API.Controllers;
using API.Data;
using API.DTOs;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Namespace;
[Authorize]
public class UsersController : BaseController
{
    private readonly IUserRepository userRepository;
    private readonly ILogger<UsersController> logger;
    private readonly IMapper mapper;


    public UsersController(IUserRepository userRepository,ILogger<UsersController> logger,IMapper mapper)
    {
        this.userRepository = userRepository;
        this.logger = logger;
        this.mapper = mapper;

    }

    [AllowAnonymous]
    [HttpGet]
    public async  Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers(){
        var users=await userRepository.GetUsersAsync();
       var usersToReturn = mapper.Map<IEnumerable<MemberDTO>>(users);
       return Ok(usersToReturn);
    }

    
    [HttpGet("{username}")]
    public  async Task<ActionResult<MemberDTO>> GetUser(string username){
    var user=await userRepository.GetUserByNameAsync(username);
     
    return mapper.Map<MemberDTO>(user);
    }
    
     [HttpPut]
     public async  Task<ActionResult> UpdatetUser(MemberUpdateDTO memberUpdateDTO){
       var userName=User.FindFirst(ClaimTypes.NameIdentifier).Value;
       var user=await userRepository.GetUserByNameAsync(userName);
       
        if(user==null)
           return NotFound();
         mapper.Map(memberUpdateDTO,user);
         if(await userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update User");
    }    

}
