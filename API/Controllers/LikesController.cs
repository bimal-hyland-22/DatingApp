using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
   
    public class LikesController : BaseController
    {
        private readonly ILogger<LikesController> _logger;
        private readonly ILikeRepository likeRepository;
        private readonly IUserRepository userRepository;

        public LikesController(ILogger<LikesController> logger, ILikeRepository likeRepository, IUserRepository userRepository)
        {
            _logger = logger;
            this.likeRepository = likeRepository;
            this.userRepository = userRepository;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username){
             var sourceUserId = User.GetUserId();
             var likeduser=await userRepository.GetUserByNameAsync(username);
             var SourceUser=await likeRepository.GetUserWithLikes(sourceUserId);
             _logger.LogInformation("source username"+SourceUser.UserName);
             if(likeduser==null)
             return NotFound();
             if(SourceUser.UserName==username)
             return BadRequest("You cant give like yourself");
             var userLike=await likeRepository.GetUserLike(sourceUserId,likeduser.Id);
             if(userLike!=null) return BadRequest("You have already liked");
             userLike =new UserLike{
                SourceUserId=sourceUserId,
                TargetUserId=likeduser.Id
             };
             SourceUser.LikedUsers.Add(userLike);
             if(await userRepository.SaveAllAsync()) 
             return Ok();
             return BadRequest("Failed to liked user");
        }
       [HttpGet]
       public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery] LikeParam likeParam){
        likeParam.UserId=User.GetUserId();
        var users=await likeRepository.GetUserLikes(likeParam);
        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages));
        return Ok(users);

       }
      
    }
}