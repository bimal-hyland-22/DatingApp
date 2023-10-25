using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        public DataContext _context ;
        private readonly ILogger<UserRepository> logger;

        public UserRepository(DataContext context,ILogger<UserRepository> logger)
        {
            _context = context;
            this.logger = logger;
        }
        public async Task<AppUser> GetUserByIdAsync(int id)
        {
          return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByNameAsync(string name)
        {
             return await _context.Users.Include(p=>p.Photos).SingleOrDefaultAsync(user=> user.UserName == name);
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
    }
}