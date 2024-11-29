using AuthService.Application.Contracts;

namespace Eventure.API.Tests.Factory;

public static class EventureFactory
{
    public static LoginUserRequest GetLoginUserRequest()
    {
        return new LoginUserRequest("321321@mail.ru", "vasya2004");
    }

    public static RegisterUserRequest GetRegisterUserRequest()
    {
        return new RegisterUserRequest("vasya", "321321@mail.ru", "vasya2004");
    }
    
    public static  List<RegisterUserRequest> GetRegisterUsersRequest()
    {
        return new List<RegisterUserRequest>
        {
            new ("Vasya", "321321@mail.ru", "vasya2004"),
            new ("Maks", "231r512345@mail.ru", "Maks2004")
        };
        
    }
}