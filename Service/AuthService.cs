using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using jwtaccount.Domain.Contracts;
using jwtaccount.Domain.Entities;
using jwtaccount.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace jwtaccount.Service
{
    public class AuthService(ApplicationDBContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<UserResponse?> GetProfile(GetUserInfo request)
        {
            var user = await context.users.FirstOrDefaultAsync(X => X.Id == request.Id);
            if (user == null)
            {
                return null;
            }
            var userResponse = new UserResponse
            {
                Id = user.Id,
                UserName = user.UserName
            };

            return userResponse;
        }

        public async Task<LoginUserResponse> LoginAsync(RegisterUser request)
        {
            var user = await context.users.FirstOrDefaultAsync(user => user.UserName == request.UserName);
            if (user == null)
            {
                return null;
            }
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.Password, request.Password) == PasswordVerificationResult.Failed)
            {
                return null;
            }
            string token = CreateToken(user);
            var userResponse = new LoginUserResponse{
                Id = user.Id,
                UserName = user.UserName,
                AccessToken = token,
                RefreshToken = await GenerateRefreshTokenAsync(user)
            };
            return userResponse;
        }

        public async Task<User?> Register(RegisterUser request)
        {
            if (await context.users.AnyAsync(user => user.UserName == request.UserName))
            {
                return null;
            }
            var user = new User();
            var hashedPassord = new PasswordHasher<User>().HashPassword(user, request.Password);
            user.UserName = request.UserName;
            user.Password = hashedPassord;
            context.users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }
        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(random);
            return Convert.ToBase64String(random);
        }
        private async Task<string>GenerateRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await context.SaveChangesAsync();
            return refreshToken;
        }
        private string CreateToken(User user)
        {
            var claims = new List<Claim>{
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Role,user.Role)
             };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var tokenDescriptor = new JwtSecurityToken(
               issuer: configuration.GetValue<string>("AppSettings:Issuer"),
               audience: configuration.GetValue<string>("AppSettings:Audience"),
               claims: claims,
               expires: DateTime.UtcNow.AddDays(1),
               signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

       private async Task<User?> ValidateRefreshToken(Guid Id,string refreshToken)
       {

        var user = await context.users.FindAsync(Id);
        if(user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow )
        {
            return null;
        }
        return user;
       }

        public async Task<TokenResponse> RefreshTokenAsync(RequestRefreshToken request)
        {
            var user = await ValidateRefreshToken(request.Id,request.RefreshToken);
            if(user is null)
            {
                return null;
            }
            var response = new TokenResponse
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateRefreshTokenAsync(user)
            };
            return response;
        }
    }
}