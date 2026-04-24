using Microsoft.AspNetCore.Mvc;
using CarDealershipAPI.Data;
using CarDealershipAPI.Models;
using System.Linq;

namespace CarDealershipAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto user)
        {
            try
            {
                Console.WriteLine("INPUT USER: " + user?.Username);
                Console.WriteLine("INPUT PASS: " + user?.Password);

                var existingUser = _context.Users
                    .FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);

                if (existingUser == null)
                    return Unauthorized("Invalid credentials");

                Console.WriteLine("FOUND USER: " + existingUser.Username);
                Console.WriteLine("ROLE: " + existingUser.Role);

                return Ok(new
                {
                    username = existingUser.Username,
                    role = existingUser.Role
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("🔥 ERROR: " + ex.ToString());
                return StatusCode(500, ex.Message);
            }
        }
    }
}