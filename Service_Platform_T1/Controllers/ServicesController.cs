using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service_Platform_T1.Data;
using Service_Platform_T1.Entities;

namespace Service_Platform_T1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServicesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var services = _context.Services.ToList();
            return Ok(services);
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateService([FromBody] Service service)
        {
            if (service == null) return BadRequest();

            _context.Services.Add(service);
            _context.SaveChanges();

            return Ok(new { message = "Service is added sucsessfully", data = service });
        }
    }
}