using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using jwtaccount.Domain.Entities;
using jwtaccount.Domain.Contracts;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using jwtaccount.Service;
namespace jwtaccount.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase

    {
        public static User user = new();
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegisterUser request)
        {
            var user = await authService.Register(request);
            if(user == null){
                return BadRequest(
                    "User exist"
                );
            }
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(RegisterUser request)
        {
            var token = await authService.LoginAsync(request);
            if (token is null){
                return BadRequest("Invalid Username");
            }

        return Ok(token);
        }
    }

    
}