using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using trackr_api.Data;
using trackr_api.Filters;
using trackr_api.Model;

namespace trackr_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : BaseController
    {
        private readonly TrackrDbContext _context;

        public CustomersController(TrackrDbContext context, ILogger<CustomersController> logger) : base (logger)
        {
            _context = context;
        }
        

        // Configure the JsonSerializer options for circular reference handling
        private JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve, // Handle circular references
            WriteIndented = true // Optional: Makes the output more readable
        };

       
        [HttpGet]
        [ServiceFilter(typeof(AuthFilter))]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                var customers = await _context.Customers
                    .Include(customer => customer.User)
                    .ThenInclude(user => user.Role)
                    .ToListAsync();
                if (customers == null)
                {
                    return NotFound("No customers found.");
                }
                else
                {
                    var jsonResponse = customers.Select(customer => new
                    {
                        CustomerId = customer.CustomerId,
                        CustomerName = customer.Name,
                        CustomerPicContact = customer.PicContact,
                        CustomerAddress = customer.Address,
                        CustomerCreatedAt = customer.CreatedAt,
                        CustomerUser = new
                        {
                            UserId = customer.User.UserId,
                            UserUsername = customer.User.Username,
                            UserCreatedAt = customer.User.CreatedAt,
                            UserRoleName = customer.User.Role.RoleName
                        }
                    });
                    return Ok(JsonSerializer.Serialize(jsonResponse, _jsonSerializerOptions));
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }

        }

        [HttpGet("{customer_id}")]
        [ServiceFilter(typeof(AuthFilter))]
        public IActionResult GetCustomer(int customer_id)
        {
            try
            {
                var customer = _context.Customers
                    .Include(customer => customer.User)
                    .ThenInclude(user => user.Role)
                    .FirstOrDefault(customer => customer.CustomerId == customer_id);

                if (customer == null)
                {
                    return NotFound($"Customer with id {customer_id} not found.");
                }
                else
                {
                    var jsonResponse = new
                    {
                        CustomerId = customer.CustomerId,
                        CustomerName = customer.Name,
                        CustomerPicContact = customer.PicContact,
                        CustomerAddress = customer.Address,
                        CustomerCreatedAt = customer.CreatedAt,
                        CustomerUser = new
                        {
                            UserId = customer.User.UserId,
                            UserUsername = customer.User.Username,
                            UserCreatedAt = customer.User.CreatedAt,
                            UserRoleName = customer.User.Role.RoleName
                        }
                    };
                    return Ok(JsonSerializer.Serialize(jsonResponse, _jsonSerializerOptions));
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
            
        }

        [HttpDelete("{customer_id}")]
        [ServiceFilter(typeof(AuthFilter))]
        public IActionResult DeleteCustomer(int customer_id)
        {
            try
            {
                var customer = _context.Customers.Find(customer_id);
                if (customer == null)
                {
                    return NotFound($"Customer with id {customer_id} not found.");
                }
                else
                {
                    _context.Customers.Remove(customer);
                    if (_context.SaveChanges() > 0)
                    {
                        return Ok($"Customer with id {customer_id} deleted.");
                    }
                    else
                    {
                        return BadRequest("Error occurred while deleting the customer.");
                    }
                }
            }
            catch (Exception ex) {
                return HandleError(ex);
            }
           
        }

        [HttpPost]
        [ServiceFilter(typeof(AuthFilter))]
        public IActionResult CreateCustomer([FromBody] Customer new_customer)
        {
            try {
                Customer customer = new Customer
                {
                    Name = new_customer.Name,
                    PicContact = new_customer.PicContact,
                    Address = new_customer.Address,
                    UserId = new_customer.UserId
                };
                _context.Customers.Add(customer);
                if (_context.SaveChanges() > 0)
                {
                    return Ok($"Customer {customer.CustomerId} created!");
                }
                else
                {
                    return BadRequest("Customer creation failed.");
                }
            }catch (Exception ex){
                return HandleError(ex);
            }
           
        }

        [HttpPatch("{customer_id}")]
        [ServiceFilter(typeof (AuthFilter))]
        public IActionResult UpdateCustomer(int customer_id, [FromBody] Customer update_customer)
        {
            try
            {
                var customer = _context.Customers.Find(customer_id);
                if (customer == null)
                {
                    return NotFound($"Customer with id {customer_id} not found.");
                }
                else
                {
                    customer.Name = update_customer.Name;
                    customer.PicContact = update_customer.PicContact;
                    customer.Address = update_customer.Address;
                    customer.UserId = update_customer.UserId;
                    _context.Customers.Update(customer);
                    if (_context.SaveChanges() > 0)
                    {
                        return Ok($"Customer {customer.CustomerId} updated!");
                    }
                    else
                    {
                        return BadRequest("Error occurred while updating the customer.");
                    }
                }
            }
            catch (Exception ex) {
                return HandleError(ex);
            }
            
        }

        [HttpGet("{customer_id}/Vehicles")]
        [ServiceFilter(typeof(AuthFilter))]
        public IActionResult GetVehicles(int customer_id)
        {
            try
            {
                var customer = _context.Customers
                .Include(customer => customer.Vehicles) // Eager load Vehicles
                .Include(customer => customer.User)
                .ThenInclude(user => user.Role)
                .FirstOrDefault(customer => customer.CustomerId == customer_id);

                if (customer == null)
                {
                    return NotFound($"Customer with ID {customer_id} not found.");
                }

                var jsonResponse = new
                {
                    Customer = new
                    {
                        CustomerId = customer.CustomerId,
                        CustomerName = customer.Name,
                        CustomerPicContact = customer.PicContact,
                        CustomerAddress = customer.Address,
                        CustomerCreatedAt = customer.CreatedAt,
                        CustomerUser = new
                        {
                            UserId = customer.User.UserId,
                            UserUsername = customer.User.Username,
                            UserCreatedAt = customer.User.CreatedAt,
                            UserRoleName = customer.User.Role.RoleName
                        }
                    },
                    Vehicles = customer.Vehicles.Select(vehicle => new
                    {
                        VehicleId = vehicle.VehicleId,
                        VehicleRegistrationNumber = vehicle.RegistrationNumber,
                        VehicleModel = vehicle.Model,
                        VehicleBrand = vehicle.Brand,
                        VehicleMilleage = vehicle.Milleage,
                        VehicleCreatedAt = vehicle.CreatedAt
                    })
                };
                return Ok(JsonSerializer.Serialize(jsonResponse, _jsonSerializerOptions));
            }
            catch (Exception ex) {
                return HandleError(ex);
            }
            
        }
    }
}
