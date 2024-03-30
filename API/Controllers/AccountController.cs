using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        private readonly IMapper _mapper;

        public AccountController(DataContext context,ITokenService tokenService, IMapper mapper)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
        }
      [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO){
            if(await UserExits(registerDTO.UserName)) return BadRequest("UserName already exists");
           
            var user=_mapper.Map<AppUser>(registerDTO);
            using var hmac=new HMACSHA512();
           
                user.UserName=registerDTO.UserName.ToLower();
               user.PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
                user.PasswordSalt=hmac.Key;
          
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDTO(){
                  UserName=user.UserName,
                  Token=_tokenService.CreateToken(user),
                  KnownAs=user.KnownAs,
                  Gender=user.Gender,
            };
        }

          [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO){

            var user=await _context.Users.Include(x=>x.Photos).
            SingleOrDefaultAsync(x=>x.UserName==loginDTO.UserName);
            if(user==null)
              return Unauthorized("Invalid UserName");
            using var hmac=new HMACSHA512(user.PasswordSalt);
            var computeHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
            for(int i=0;i<computeHash.Length;i++){
                if(computeHash[i]!=user.PasswordHash[i])
                  return Unauthorized("Invalid Password");
            }
          return new UserDTO(){
                  UserName=user.UserName,
                  Token=_tokenService.CreateToken(user),
                  PhotoUrl=user.Photos.Count>0? user.Photos.FirstOrDefault(x=>x.IsMain==true).Url:"",
                  KnownAs=user.KnownAs,
                  Gender=user.Gender
            };

        }

        private async Task<bool> UserExits(string userName){
            return await _context.Users.AnyAsync(user=>user.UserName==userName.ToLower());
        }
    }
}