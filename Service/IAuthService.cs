using jwtaccount.Domain.Contracts;
using jwtaccount.Domain.Entities;

namespace  jwtaccount.Service
{
    public interface IAuthService
    {
        Task<User?>Register(RegisterUser request);
        Task<string>LoginAsync(RegisterUser request);
    }
}