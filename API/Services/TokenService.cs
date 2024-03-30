
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    { 
        private readonly SymmetricSecurityKey _key;  
        public TokenService(IConfiguration configuration)
        {
            _key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"]));
        }
        public string CreateToken(AppUser user)
        {
            var claims=new List<Claim> 
            {
                new Claim(JwtRegisteredClaimNames.NameId,user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName,user.UserName)
            };
            var cred=new SigningCredentials(_key,SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor=new SecurityTokenDescriptor
            {
                  Subject=new ClaimsIdentity(claims),
                  Expires=DateTime.Now.AddDays(7),
                  SigningCredentials=cred
            };
            var tokenHandler=new JwtSecurityTokenHandler();
            var token=tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}