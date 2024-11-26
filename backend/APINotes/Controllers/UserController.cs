using APINotes.Data;
using Microsoft.AspNetCore.Mvc;
using APINotes.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace APINotes.Controllers
{
    [Route("api/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;

        public UserController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("user/login")]
        public async Task<ActionResult<User>> Login(User loginUser)
        {
            var user = await _context.Users
                .Where(i => i.Username == loginUser.Username && i.IsActive).FirstOrDefaultAsync();

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginUser.Password, user.Password)) return BadRequest("Invalid credentials");

            return Ok(new { user!.Username, user.NotesCreated, user.NotesArchived });
        }

        [HttpPost("user/create")]
        public async Task<ActionResult> CreateUser(User newUser)
        {
            if (string.IsNullOrEmpty(newUser.Password)) return BadRequest("Invalid credentials");

            var existingUser = await _context.Users.FirstOrDefaultAsync(i => i.Username == newUser.Username && i.IsActive);

            if (existingUser != null) return Conflict("User already exists");

            newUser.Id = Guid.NewGuid();

            newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok("User created");
        }
    }
}
