using Service_Platform_T1.DTOs;
using Service_Platform_T1.Entities;
using Service_Platform_T1.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Service_Platform_T1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            // 1. نتأكد إن الإيميل مش متكرر
            if (await _repo.UserExists(dto.Email))
                return BadRequest("Email already exists");

            // 2. (منطق الأمان) نحدد الرتبة بناءً على اختيار المستخدم
            // لو هو اختار "Company" -> نخليه Company
            // لو اختار أي حاجة تانية (أو حاول يكتب Admin) -> نخليه Client غصب عنه
            string roleToAssign = "Client";
            if (dto.UserRole == 1)
            {
                roleToAssign = "Company";
            }

            // 3. تجهيز بيانات اليوزر
            var userToCreate = new User
            {
                UserName = dto.Email, // مهم جداً: Identity بيحتاج ده ضروري وإلا هيضرب Error
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber
                // لاحظي: شلنا الـ Role من هنا لأننا هنضيفه في الخطوة الجاية عن طريق Identity
            };

            // 4. التسجيل الفعلي
            var createdUser = await _repo.Register(userToCreate, dto.Password);

            // ملحوظة: لحد هنا اليوزر اتعمل، بس لسه الرتبة (roleToAssign) متخزنتش في الجدول
            // عشان تتخزن، محتاجين نعدل الـ Repository في الخطوة الجاية ياخد الرتبة دي ويضيفها.
            // بس حالياً الكود ده سليم وهيعمل اليوزر من غير مشاكل.

            if (createdUser != null)
            {
                return Ok(new { message = "User registered successfully", userId = createdUser.Id, role = roleToAssign });
            }

            return BadRequest("Registration failed");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _repo.Login(dto.Email, dto.Password);

            if (token == null)
                return Unauthorized("Invalid email or password");

            return Ok(new { token });
        }
    }
}
//using Service_Platform_T1.DTOs;
//using Service_Platform_T1.Entities;
//using Service_Platform_T1.Interfaces;
//using Microsoft.AspNetCore.Mvc;

//namespace Service_Platform_T1.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthController : ControllerBase
//    {
//        private readonly IAuthRepository _repo; 

//        public AuthController(IAuthRepository repo)
//        {
//            _repo = repo;
//        }

//        [HttpPost("register")]
//        public async Task<IActionResult> Register(RegisterDto dto)
//        {
//            if (await _repo.UserExists(dto.Email))
//                return BadRequest("Email already exists");

//            var userToCreate = new User
//            {
//                Email = dto.Email,
//                FullName = dto.FullName,
//                PhoneNumber = dto.PhoneNumber,
             
//            };

//            var createdUser = await _repo.Register(userToCreate, dto.Password);

//            return Ok(new { message = "User registered successfully", userId = createdUser.Id });
//        }

//        [HttpPost("login")]
//        public async Task<IActionResult> Login(LoginDto dto)
//        {
//            var token = await _repo.Login(dto.Email, dto.Password);

//            if (token == null)
//                return Unauthorized("Invalid email or password");

//            return Ok(new { token });
//        }
//    }
//}
