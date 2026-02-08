using Microsoft.AspNetCore.Identity;

namespace Service_Platform_T1.Entities
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}






//namespace Service_Platform_T1.Entities
//{
//    public class User
//    {
//            public int Id { get; set; }
//            public string FullName { get; set; } = string.Empty;
//            public string Email { get; set; } = string.Empty;
//            public string PasswordHash { get; set; } = string.Empty; 
//            public string PhoneNumber { get; set; } = string.Empty;
//            public string Role { get; set; } = "Client"; 

//    }
//}

