using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository: IUserRepository
    {
        public DataContext _context ;
        private readonly ILogger<UserRepository> logger;

        private readonly IMapper mapper;
        public UserRepository(DataContext context,ILogger<UserRepository> logger, IMapper _mapper)
        {
            _context = context;
            this.logger = logger;
            mapper = _mapper;
        }
        public async Task<AppUser> GetUserByIdAsync(int id)
        {
          return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByNameAsync(string name)
        {
             var user= await _context.Users.Include(p=>p.Photos).SingleOrDefaultAsync(user=> user.UserName == name);
             logger.LogError("Inside get user"+user.UserName);

             return user;
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users.Include(p=>p.Photos).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
           return await _context.SaveChangesAsync() > 0;
        }

        public async void Update(AppUser user)
        {
            _context.Entry(user).State=EntityState.Modified;
        }

        public async Task<PagedList<MemberDTO>> GetMemebrAsync(UserParams userParams)
        {
          var query=_context.Users.AsQueryable();
          query=query.Where(u=>u.UserName!=userParams.CurrentUserName);
          if(userParams.Gender!="all"){
          query=query.Where(u=>u.Gender==userParams.Gender);
          }
          var minDob=DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge));
          var maxDob=DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));
          query=query.Where(u=> u.DateOfBirth.Year >=minDob.Year && u.DateOfBirth.Year<=maxDob.Year);
          query=userParams.OrderBy switch
          {
               "created" =>query.OrderByDescending(u=>u.Created),
               _=>query.OrderByDescending(u=>u.LastActive)
          };

           return await PagedList<MemberDTO>.ToPagedList(query.AsNoTracking().ProjectTo<MemberDTO>(mapper.ConfigurationProvider),userParams.PageNumber,userParams.PageSize);

        }

        public async Task<MemberDTO> GetMemebrAsync(string username)
        {
             return await _context.Users.Where(x=>x.UserName==username).ProjectTo<MemberDTO>(mapper.ConfigurationProvider).SingleOrDefaultAsync();
        }
       
    }

 
}