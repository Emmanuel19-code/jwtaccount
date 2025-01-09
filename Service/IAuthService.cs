using jwtaccount.Domain.Contracts;
using jwtaccount.Domain.Entities;

namespace  jwtaccount.Service
{
    public interface IAuthService
    {
        Task<User?>Register(RegisterUser request);
        Task<LoginUserResponse>LoginAsync(RegisterUser request);
        Task<UserResponse?>GetProfile(GetUserInfo request);
        Task<TokenResponse>RefreshTokenAsync(RequestRefreshToken request);
    }
}