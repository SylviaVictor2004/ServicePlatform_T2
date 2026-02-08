using Service_Platform_T1.Entities;

namespace Service_Platform_T1.Interfaces
{
    public interface IAuthRepository
    {
       
        Task<User> Register(User user, string password);
   
        Task<string> Login(string email, string password);

        Task<bool> UserExists(string email);
    }
}
