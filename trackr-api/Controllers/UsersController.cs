using Microsoft.AspNetCore.Mvc;
using trackr_api.Model;
using trackr_api.Data;
using Microsoft.EntityFrameworkCore;

namespace trackr_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly TrackrDbContext _context;
        public UsersController(TrackrDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllUser()
        {
            var users = await _context.Users.ToListAsync();
            if (users == null)
            {
                return NotFound("users not found");
            }
            else {
                return Ok(users);

            }
        }

        [HttpGet("{user_id}")]
        public IActionResult GetUser(int user_id)
        {
            var user = _context.Users.Find(user_id);
            if (user == null)
            {
                return NotFound();
            }
            else {
                return Ok(user);
            }
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] User new_user)
        {
            User user = new User
            {
                Username = new_user.Username,
                RoleId = new_user.RoleId,
                PasswordHash = new_user.PasswordHash
            };
            _context.Users.Add(user);

            if (_context.SaveChanges() > 0) 
            {
                return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
            }
            else
            {
                return BadRequest("User not created. Something went wrong.");
            }
        }


        [HttpPatch("user_id")]
        public IActionResult UpdateUser(int user_id, [FromBody] User updated_user)
        {
            var user = _context.Users.Find(user_id);
            if (user == null)
            {
                return NotFound($"User with is {user_id} not found");
            }
            else { 
                user.Username = updated_user.Username;
                user.PasswordHash = updated_user.PasswordHash;
                user.RoleId = updated_user.RoleId;
                user.ModifiedAt = DateTime.Now;
                _context.Users.Update(user);
                if (_context.SaveChanges() > 0)
                {
                    return Ok($"User {user.UserId} updated successfully");
                }
                else
                {
                    return BadRequest($"User {user.UserId} not updated. Something went wrong.");
                }
            }            
        }


        [HttpDelete("{user_id}")]
        public IActionResult DeleteUser(int user_id)
        {
            var user = _context.Users.Find(user_id);
            if (user == null)
            {
                return NotFound($"User {user_id} not found");
            }
            else { 
                _context.Users.Remove(user);
                if (_context.SaveChanges() > 0)
                {
                    return Ok($"User Deleted: {user}");
                }
                else {
                    return BadRequest($"User {user.UserId} not deleted. Something went wrong");
                }
            }
        }

    }
}
