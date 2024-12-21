using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using trackr_api.Data;
using trackr_api.Model;

namespace trackr_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : Controller
    {
        private readonly TrackrDbContext _context;
        public CustomersController(TrackrDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task <ActionResult<List<User>>> GetAllCustomers()
        {
            var customers = await _context.Customers.ToListAsync();
            if (customers == null)
            {
                return NotFound("No customrs found");
            }
            else 
            {
                return Ok(customers);
            }
        }

        [HttpGet("{customer_id}")]
        public IActionResult GetCustomer(int customer_id)
        {
            var customer = _context.Customers.Find(customer_id);
            if (customer == null)
            {
                return NotFound($"Customer with id {customer_id} not found");
            }
            else 
            {
                return Ok(customer);
            }
        }

        [HttpDelete("{customer_id}")]
        public IActionResult DeleteCustomer(int customer_id)
        {
            var customer = _context.Customers.Find(customer_id);
            if (customer == null)
            {
                return NotFound($"Customer with id {customer_id} not found");
            }
            else {
                _context.Customers.Remove(customer);
                if (_context.SaveChanges() > 0)
                {
                    return Ok($"Customer data Deleted: {customer_id}");
                }
                else {
                    return BadRequest($"Customer with id {customer_id} not deleted. Something went wrong");
                }
            }
        }

        [HttpPost]
        public IActionResult CreateCustomer([FromBody]Customer new_customer)
        {
            Customer customer = new Customer();
            customer.Name = new_customer.Name;
            customer.PicContact = new_customer.PicContact;
            customer.Address = new_customer.Address;
            customer.UserId = new_customer.UserId;
            _context.Customers.Add(customer);
            if (_context.SaveChanges() > 0)
            {
                return Ok($"Customer {customer.CustomerId} created!");
            }
            else 
            {
                return BadRequest("Customer failed to create. Something went wrong");
            }
        }


        [HttpPatch("{customer_id}")]
        public IActionResult UpdateCustomer(int customer_id, [FromBody] Customer update_customer)
        {
            var customer = _context.Customers.Find(customer_id);
            if (customer == null)
            {
                return NotFound($"Customer with id {customer_id} not found");
            }
            else {
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
                    return BadRequest("Customer failed to update. Something went wrong");
                }
            } 
        }
    }
}
