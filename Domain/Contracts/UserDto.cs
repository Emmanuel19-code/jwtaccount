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
        public string UserName {get;set;}
    }
}