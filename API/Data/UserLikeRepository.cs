using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserLikeRepository : ILikeRepository
    {

        private readonly DataContext _context;

        public UserLikeRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
        {
           return await _context.Likes.FindAsync(sourceUserId, targetUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikeParam likeParam)
        {
           var users=_context.Users.OrderBy(u=>u.UserName).AsQueryable();
           var likes=_context.Likes.AsQueryable();
           if(likeParam.Predicate=="liked"){
            likes=likes.Where(like=>like.SourceUserId==likeParam.UserId);
            users=likes.Select(like=>like.TargetUser);
           }
           if(likeParam.Predicate=="likedBy"){
             likes=likes.Where(like=>like.TargetUserId==likeParam.UserId);
            users=likes.Select(like=>like.SourceUser);
           }

           var LikedUsers= users.Select(user=>new LikeDto{
            UserName =user.UserName,
            KnownAs=user.KnownAs,
            City=user.City,
            PhotoUrl=user.Photos.FirstOrDefault(x=>x.IsMain).Url,
            Id=user.Id,
            Age=user.DateOfBirth.CalculateAge()
           });
           return await PagedList<LikeDto>.ToPagedList(LikedUsers,likeParam.PageNumber,likeParam.PageSize);

        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users.Include(x=>x.LikedUsers).FirstOrDefaultAsync(x=>x.Id==userId);
        }
    }
}