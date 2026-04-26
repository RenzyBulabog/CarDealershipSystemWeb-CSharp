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

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            // 🔥 check if username exists
            if (_context.Users.Any(u => u.Username == user.Username))
                return BadRequest("Username already exists");

             // 🔥 FORCE ROLE (IMPORTANT)
            if (user.Role != "admin")
                user.Role = "customer";

             _context.Users.Add(user);
             _context.SaveChanges();

            return Ok(user);
        }
    }
}