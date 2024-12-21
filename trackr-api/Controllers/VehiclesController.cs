using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using trackr_api.Model;
using trackr_api.Data;

namespace trackr_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiclesController : Controller
    {
        private readonly TrackrDbContext _context;
        public VehiclesController(TrackrDbContext context)
        {
            _context = context;
        }

        //return all vehicles (admin)
        [HttpGet]
        public async Task<ActionResult<List<Vehicle>>> GetAllVehicles() 
        {
            var vehicles = await _context.Vehicles.ToListAsync();
            if (vehicles == null)
            {
                return NotFound("Vehicle list not found");
            }
            else { 
                return Ok(vehicles);
            }
        }
        [HttpGet("{vehicle_id}")]
        public IActionResult GetVehicle(int vehicle_id)
        {
            var vehicle = _context.Vehicles.Find(vehicle_id);
            if (vehicle == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(vehicle);
            }
        }


        [HttpPost]
        public IActionResult CreateVehicle([FromBody] Vehicle new_vehicle)
        {
            Vehicle vehicle= new Vehicle
            {
                RegistrationNumber = new_vehicle.RegistrationNumber,
                Model = new_vehicle.Model,
                Brand = new_vehicle.Brand,
                Milleage = new_vehicle.Milleage,
                CustomerId = new_vehicle.CustomerId
            };
            _context.Vehicles.Add(new_vehicle);

            if (_context.SaveChanges() > 0)
            {
                return CreatedAtAction(nameof(GetVehicle), new { id = vehicle.VehicleId }, vehicle);
            }
            else
            {
                return BadRequest("Vehicle not created. Something went wrong.");
            }
        }

        [HttpPatch("vehicle_id")]
        public IActionResult UpdateVehicle(int vehicle_id, [FromBody] Vehicle updated_vehicle)
        {
            var vehicle = _context.Vehicles.Find(vehicle_id);
            if (vehicle == null)
            {
                return NotFound($"Vehicle with is {vehicle_id} not found");
            }
            else
            {
                vehicle.RegistrationNumber = updated_vehicle.RegistrationNumber;
                vehicle.Model = updated_vehicle.Model;
                vehicle.Brand = updated_vehicle.Brand;
                vehicle.Milleage = updated_vehicle.Milleage;
                vehicle.CustomerId = updated_vehicle.CustomerId;
                vehicle.ModifiedAt = DateTime.Now;
                _context.Vehicles.Update(vehicle);
                if (_context.SaveChanges() > 0)
                {
                    return Ok($"Vehicle {vehicle.VehicleId} updated successfully");
                }
                else
                {
                    return BadRequest($"Vehicle {vehicle.VehicleId} not updated. Something went wrong.");
                }
            }
        }

        [HttpDelete("{vehicle_id}")]
        public IActionResult DeleteVehicle(int vehicle_id)
        {
            var vehicle = _context.Vehicles.Find(vehicle_id);
            if (vehicle == null)
            {
                return NotFound($"Vehicle {vehicle_id} not found");
            }
            else
            {
                _context.Vehicles.Remove(vehicle);
                if (_context.SaveChanges() > 0)
                {
                    return Ok($"Vehicle Deleted: {vehicle}");
                }
                else
                {
                    return BadRequest($"Vehicle {vehicle.VehicleId} not deleted. Something went wrong");
                }
            }
        }
    }
}
