using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Service_Platform_T1.Data;
using Service_Platform_T1.Entities;
using Service_Platform_T1.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Service_Platform_T1.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public AuthRepository(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<string> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            {
                return null;
            }

            return await CreateToken(user);
        }

        public async Task<User> Register(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                return user;
            }
            return null;
        }

        public async Task<bool> UserExists(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        private async Task<string> CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName ?? user.Email) 
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}

//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Service_Platform_T1.Data;
//using Service_Platform_T1.Entities;
//using Service_Platform_T1.Interfaces;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace Service_Platform_T1.Repositories

//{
//    public class AuthRepository : IAuthRepository
//    {
//        private readonly AppDbContext _context;
//        private readonly IConfiguration _configuration; 

//        public AuthRepository(AppDbContext context, IConfiguration configuration)
//        {
//            _context = context;
//            _configuration = configuration;
//        }

//        public async Task<User> Register(User user, string password)
//        {

//            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

//            await _context.Users.AddAsync(user);
//            await _context.SaveChangesAsync();
//            return user;
//        }

//        public async Task<string> Login(string email, string password)
//        {
//            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

//            if (user == null) return null;

//            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return null; 

//            return CreateToken(user); 
//        }

//        public async Task<bool> UserExists(string email)
//        {
//            return await _context.Users.AnyAsync(x => x.Email == email);
//        }


//        private string CreateToken(User user)
//        {
//            var claims = new List<Claim>
//            {
//                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//                new Claim(ClaimTypes.Email, user.Email),
//                new Claim(ClaimTypes.Role, user.Role)
//            };

//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!));
//            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
//            var tokenDescriptor = new SecurityTokenDescriptor
//            {
//                Subject = new ClaimsIdentity(claims),
//                Expires = DateTime.Now.AddDays(1),
//                SigningCredentials = creds
//            };
//            var tokenHandler = new JwtSecurityTokenHandler();
//            var token = tokenHandler.CreateToken(tokenDescriptor);
//            return tokenHandler.WriteToken(token);
//        }
//    }
//}