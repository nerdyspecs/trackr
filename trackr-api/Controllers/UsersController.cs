using Microsoft.AspNetCore.Mvc;
using trackr_api.Model;
using trackr_api.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System;
using trackr_api.Filters;


namespace trackr_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : BaseController
    {
        private readonly TrackrDbContext _context;

        public UsersController(TrackrDbContext context, ILogger<UsersController> logger)
            : base(logger) 
        {
            _context = context;
        }

        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
            WriteIndented = true
        };

        [HttpGet]
        [ServiceFilter(typeof(AuthFilter))]
        public async Task<IActionResult> GetAllUser()
        {
            try
            {
                var users = await _context.Users.Include(user => user.Role).ToListAsync();
                if (users == null || users.Count == 0)
                    return NotFound("Users not found");

                var jsonResponse = users.Select(user => new
                {
                    UserId = user.UserId,
                    UserUsername = user.Username,
                    UserRole = new
                    {
                        RoleId = user.Role.RoleId,
                        RoleName = user.Role.RoleName
                    },
                    UserCreatedAt = user.CreatedAt,
                    UserModifiedAt = user.ModifiedAt
                });

                return Ok(JsonSerializer.Serialize(jsonResponse, _jsonSerializerOptions));
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [HttpGet("{user_id}")]
        [ServiceFilter(typeof(AuthFilter))]
        public IActionResult GetUser(int user_id)
        {
            try
            {
                var user = _context.Users
                    .Include(user => user.Role)
                    .FirstOrDefault(user => user.UserId == user_id);

                if (user == null)
                    return NotFound($"User {user_id} not found");

                var jsonResponse = new
                {
                    UserId = user.UserId,
                    UserUsername = user.Username,
                    UserRole = new
                    {
                        RoleId = user.Role.RoleId,
                        RoleName = user.Role.RoleName
                    },
                    UserCreatedAt = user.CreatedAt,
                    UserModifiedAt = user.ModifiedAt
                };

                return Ok(JsonSerializer.Serialize(jsonResponse, _jsonSerializerOptions));
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(AuthFilter))]
        public IActionResult CreateUser([FromBody] User new_user)
        {
            try
            {
                var user = new User
                {
                    Username = new_user.Username,
                    RoleId = new_user.RoleId,
                    PasswordHash = new_user.PasswordHash
                };
                _context.Users.Add(user);
                _context.SaveChanges();

                return CreatedAtAction(nameof(GetUser), new { user_id = user.UserId }, user);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [HttpPatch("{user_id}")]
        [ServiceFilter(typeof(AuthFilter))]
        public IActionResult UpdateUser(int user_id, [FromBody] User updated_user)
        {
            try
            {
                var user = _context.Users.Find(user_id);
                if (user == null)
                    return NotFound($"User {user_id} not found");

                user.Username = updated_user.Username;
                user.PasswordHash = updated_user.PasswordHash;
                user.RoleId = updated_user.RoleId;
                user.ModifiedAt = DateTime.Now;
                _context.Users.Update(user);
                _context.SaveChanges();

                return Ok($"User {user.UserId} updated successfully");
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [HttpDelete("{user_id}")]
        [ServiceFilter(typeof(AuthFilter))]
        public IActionResult DeleteUser(int user_id)
        {
            try
            {
                var user = _context.Users.Find(user_id);
                if (user == null)
                    return NotFound($"User {user_id} not found");

                _context.Users.Remove(user);
                _context.SaveChanges();

                return Ok($"User {user_id} deleted successfully");
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
    }
}
