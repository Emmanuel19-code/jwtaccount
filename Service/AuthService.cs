using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using jwtaccount.Domain.Contracts;
using jwtaccount.Domain.Entities;
using jwtaccount.Infrastructure.Context;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace jwtaccount.Service
{
    public class AuthService(ApplicationDBContext context,IConfiguration configuration) : IAuthService
    {
        public async Task<string> LoginAsync(RegisterUser request)
        {
            var user = await context.users.FirstOrDefaultAsync(user=>user.UserName == request.UserName);
            if(user == null){
                return null;
            }
            if(new PasswordHasher<User>().VerifyHashedPassword(user,user.Password,request.Password )== PasswordVerificationResult.Failed)
            {
                return null;
            }
            string token = CreateToken(user);
            throw new NotImplementedException();
        }

        public async Task<User?> Register(RegisterUser request)
        {
            if(await context.users.AnyAsync(user=>user.UserName == request.UserName)){
                return null;
            }
            var user = new User();
            var hashedPassord = new PasswordHasher<User>().HashPassword(user,request.Password);
             user.UserName = request.UserName;
             user.Password = hashedPassord;
             context.users.Add(user);
             await context.SaveChangesAsync();
             return user;
        }
        private string CreateToken(User user){
             var claims = new List<Claim>{
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
             };
             var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

             var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha512);
             var tokenDescriptor = new JwtSecurityToken(
                issuer:configuration.GetValue<string>("AppSettings:Issuer"),
                audience:configuration.GetValue<string>("AppSettings:Audience"),
                claims:claims,
                expires:DateTime.UtcNow.AddDays(1),
                signingCredentials:creds
             );
             return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}