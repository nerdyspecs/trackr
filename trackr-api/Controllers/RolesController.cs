using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using trackr_api.Data;
using trackr_api.Model;

namespace trackr_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : Controller
    {
        private readonly TrackrDbContext _context;
        public RolesController(TrackrDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Role>>> GetAllRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            if (roles == null)
            {
                return NotFound("Roles not found");
            }
            else {
                return Ok(roles);
            }
        }

        [HttpGet("{role_id}")]
        public IActionResult GetRole(int role_id)
        {
            var role = _context.Roles.Find(role_id);
            if (role == null)
            {
                return NotFound($"Role with id {role_id} not found");
            }
            else {
                return Ok(role);
            }
        }

        [HttpPost]
        public IActionResult CreateRole([FromBody]Role new_role) 
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
            else { 
                return BadRequest("Role not created");
            }
        }

        [HttpPatch("role_id")]
        public IActionResult UpdateUser(int role_id, [FromBody] Role updated_role)
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


        [HttpDelete("{role_id}")]
        public IActionResult DeleteRole(int role_id) {
            var role = _context.Roles.Find(role_id);
            if (role == null)
            {
                return NotFound($"Role with id {role_id} not found");
            }
            else { 
                _context.Roles.Remove(role);
                if (_context.SaveChanges() > 0)
                {
                    return Ok($"Role {role.RoleId} deleted");
                }
                else { 
                    return BadRequest($"Role {role.RoleId} not deleted. Something went wrong.");
                }
            }
        }

    }
}
