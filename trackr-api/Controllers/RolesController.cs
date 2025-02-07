using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.Json;
using trackr_api.Data;
using trackr_api.Filters;
using trackr_api.Model;

namespace trackr_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : BaseController
    {
        private readonly TrackrDbContext _context;
        public RolesController(TrackrDbContext context, ILogger<RolesController> logger) : base(logger)
        {
            _context = context;
        }

        [HttpGet]
        [ServiceFilter(typeof(AuthFilter))]
        public async Task <IActionResult> GetAllRoles()
        {
            try
            {
                var roles = await _context.Roles.ToListAsync();
                if (roles == null)
                {
                    return NotFound("Roles not found");
                }
                else
                {
                    var jsonResponse = roles.Select(role => new
                    {
                        RoleId = role.RoleId,
                        RoleName = role.RoleName,
                        RoleDescription = role.RoleDescription
                    });

                    return Ok(JsonSerializer.Serialize(jsonResponse, _jsonSerializerOptions));
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
            
        }

        [HttpGet("{role_id}")]
        [ServiceFilter(typeof(AuthFilter))]
        public IActionResult GetRole(int role_id)
        {
            try
            {
                var role = _context.Roles.Find(role_id);
                if (role == null)
                {
                    return NotFound($"Role with id {role_id} not found");
                }
                else
                {
                    var jsonResponse = new
                    {
                        RoleId = role.RoleId,
                        RoleName = role.RoleName,
                        RoleDescription = role.RoleDescription
                    };

                    return Ok(JsonSerializer.Serialize(jsonResponse, _jsonSerializerOptions));
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
           
        }

        [HttpPost]
        [ServiceFilter(typeof (AuthFilter))]
        public IActionResult CreateRole([FromBody]Role new_role) 
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                Role role = new Role();
                role.RoleName = new_role.RoleName;
                role.RoleDescription = new_role.RoleDescription;
                _context.Roles.Add(role);
                if (_context.SaveChanges() > 0)
                {
                    return Ok($"Role Created");
                }
                else
                {
                    return BadRequest("Role not created");
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
           
        }

        [HttpPatch("role_id")]
        [ServiceFilter(typeof(AuthFilter))]
        public IActionResult UpdateUser(int role_id, [FromBody] Role updated_role)
        {
            try
            {
                var role = _context.Roles.Find(role_id);
                if (role == null)
                {
                    return NotFound($"Role with is {role_id} not found");
                }
                else
                {
                    role.RoleName = updated_role.RoleName;
                    role.RoleDescription = updated_role.RoleDescription;
                    role.ModifiedAt = DateTime.Now;
                    _context.Roles.Update(role);
                    if (_context.SaveChanges() > 0)
                    {
                        return Ok($"Role {role.RoleId} updated successfully");
                    }
                    else
                    {
                        return BadRequest($"Role {role.RoleId} not updated. Something went wrong.");
                    }
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        
        }

        [HttpDelete("{role_id}")]
        [ServiceFilter(typeof (AuthFilter))]
        public IActionResult DeleteRole(int role_id) {
            try
            {
                var role = _context.Roles.Find(role_id);
                if (role == null)
                {
                    return NotFound($"Role with id {role_id} not found");
                }
                else
                {
                    _context.Roles.Remove(role);
                    if (_context.SaveChanges() > 0)
                    {
                        return Ok($"Role {role.RoleId} deleted");
                    }
                    else
                    {
                        return BadRequest($"Role {role.RoleId} not deleted. Something went wrong.");
                    }
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
    }
}
