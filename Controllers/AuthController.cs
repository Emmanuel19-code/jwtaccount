using Microsoft.AspNetCore.Mvc;
using jwtaccount.Domain.Entities;
using jwtaccount.Service;
using System.Threading.Tasks;
using jwtaccount.Domain.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace jwtaccount.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        
        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        // Register User
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegisterUser request)
        {
            var user = await authService.Register(request);
            if (user == null)
            {
                // Return BadRequest with a more structured response
                return BadRequest(new { message = "User already exists" });
            }
            return Ok(user);
        }

        // Login User
        [HttpPost("login")]
        public async Task<ActionResult<LoginUserResponse>> Login(RegisterUser request)
        {
            var token = await authService.LoginAsync(request);
            if (token is null)
            {
                // Return BadRequest with a more structured response
                return BadRequest(new { message = "Invalid Username or Password" });
            }

            return Ok(token); 
        }

        // Get Profile
        [Authorize]
        [HttpPost("profile")]
        public async Task<ActionResult<UserResponse>> Profile(GetUserInfo request)
        {
            var user = await authService.GetProfile(request);
            if (user is null)
            {
                // Return NotFound with a structured response
                return NotFound(new { message = "User not found" });
            }
            return Ok(user); 
        }

        [Authorize]
        [HttpGet("greet")]
        public  IActionResult Greet()
        {
            return Ok("Hello user");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult GreetAdmin()
        {
            return Ok("Hello Admin");
        }

        [HttpPost("refreshToken")]
        public async Task<ActionResult<TokenResponse>> RefreshToken(RequestRefreshToken request)
        {
            var token = await authService.RefreshTokenAsync(request);
            if (token is null || token.AccessToken is null || token.RefreshToken is null){
                return null;
            }
            return Ok(token);
        }
    }
}
