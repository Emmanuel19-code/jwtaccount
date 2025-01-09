namespace jwtaccount.Domain.Contracts
{
    public class RegisterUser
    {
        public string UserName {get;set;}
        public string Password {get;set;}
    }
    public class GetUserInfo 
    {
        public Guid Id {get;set;}
        //public string UserName {get;set;}
    }
    public class UserResponse {
        public  Guid Id {get;set;}
        public string UserName {get;set;}
    }

    public class LoginUserResponse
    {
        public Guid Id {get;set;}
        public string UserName {get;set;}
        public string AccessToken {get;set;}
        public string RefreshToken {get;set;}
    }

    public class RequestRefreshToken 
    {
        public Guid Id {get;set;}
        public required string RefreshToken {get;set;}
    }

    public class TokenResponse {
        public string AccessToken {get;set;}
        public string RefreshToken {get;set;}
    }
}