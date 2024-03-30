using System.Security.Claims;
using API;
using API.Controllers;
using API.Data;
using API.DTOs;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Services;
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
    private readonly IPhotoService photoService;


    public UsersController(IUserRepository userRepository,ILogger<UsersController> logger,IMapper mapper,IPhotoService photoService)
    {
       
        this.userRepository = userRepository;
        this.logger = logger;
        this.mapper = mapper;
        this.photoService = photoService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async  Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers([FromQuery] UserParams userParams){
      if(userParams==null)
       return BadRequest("Failed to update User");
        logger.LogInformation("UserName "+User.GetUserName());
        var currentUser=await userRepository.GetUserByNameAsync(User.GetUserName());
        userParams.CurrentUserName=currentUser.UserName;
        if(string.IsNullOrEmpty(userParams.Gender)){
          userParams.Gender= (currentUser.Gender=="male")? "female" :"male" ;
        }
        var users=await userRepository.GetMemebrAsync(userParams);
        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages));
        
       return Ok(users);
    }

    
    [HttpGet("{username}")]
    public  async Task<ActionResult<MemberDTO>> GetUser(string username){
    return await userRepository.GetMemebrAsync(username);
    
    }
    
     [HttpPut]
     public async  Task<ActionResult> UpdatetUser(MemberUpdateDTO memberUpdateDTO){
    //    var userName=User.FindFirst(ClaimTypes.NameIdentifier).Value;
       var user=await userRepository.GetUserByNameAsync(User.GetUserName());
       
        if(user==null)
           return NotFound();
         mapper.Map(memberUpdateDTO,user);
         if(await userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update User");
    }

    [HttpPost("add-photo")]  
    public async Task<ActionResult<PhotoDTO>>  UploadPhoto(IFormFile file){
          var user=await userRepository.GetUserByNameAsync(User.GetUserName());
          if(user==null) return NotFound();

          var result =await photoService.AddPhotoAsync(file);
          if(result.Error!=null) return BadRequest(result.Error.Message);
          var photo = new Photo{
            Url=result.SecureUri.AbsoluteUri,
            PublicId=result.PublicId
          };
          if(user.Photos.Count==0)
             photo.IsMain=true;
            user.Photos.Add(photo);
           if(await userRepository.SaveAllAsync()) {
            return CreatedAtAction(nameof(GetUser),new {username=user.UserName},mapper.Map<PhotoDTO>(photo));
           }
           return BadRequest("Erro while uploading phot");  
             
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainAsync(int photoId){
       var user=await userRepository.GetUserByNameAsync(User.GetUserName());
          if(user==null) return NotFound();
      var latest=user.Photos.FirstOrDefault(p=>p.Id==photoId);
      if(latest==null) return NotFound();
      var current=user.Photos.FirstOrDefault(p=>p.IsMain==true);
      if(current!=null) current.IsMain=false;
      latest.IsMain=true;
      if(await userRepository.SaveAllAsync()) {
            return NoContent();
      }
      return BadRequest("Erro while Setting main Photo");

    }

    [HttpDelete("delete-photo/{photoId}")]
      public async Task<ActionResult> DeletePhotAsysn (int photoId){
       var user=await userRepository.GetUserByNameAsync(User.GetUserName());
          if(user==null) return NotFound();
      var photo=user.Photos.FirstOrDefault(p=>p.Id==photoId);
      if(photo==null)
      return NotFound();
      
      if(photo.IsMain==true)
      return BadRequest("You cant delete your main photo");

      if(photo.PublicId!=null){
        var result=await photoService.DeletePhotoAsync(photo.PublicId);
        if(result.Error!=null) return BadRequest(result.Error.Message);
      }
      user.Photos.Remove(photo);

      if(await userRepository.SaveAllAsync()) {
            return Ok();
      }
      return BadRequest("Erro while deleting phot");

    }

}
