using Microsoft.AspNetCore.Mvc;
using trackr_api.Data;
using trackr_api.Model;
using trackr_api.Services;

namespace trackr_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestingController : Controller
    {
        private readonly TrackrDbContext _context;
        private readonly PopulateData _populateData;

        public TestingController(TrackrDbContext context, PopulateData populateData)
        {
            _context = context;
            _populateData = populateData;
        }

        [HttpPost("dataseed")]
        public IActionResult SeedData()
        {
            try
            {
                _populateData.SeedData();  // Use the injected PopulateData instance to call SeedData
                return Ok("Data seeded successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error seeding data: {ex.Message}");  // In case of any errors, return a 400 BadRequest
            }
        }
    }
}
