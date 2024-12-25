using Microsoft.AspNetCore.Mvc;
using trackr_api.Model;
using trackr_api.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace trackr_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : Controller
    {
        private readonly TrackrDbContext _context;
        public JobsController(TrackrDbContext context)
        {
            _context = context;
        }

        // Configure the JsonSerializer options for circular reference handling
        private JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve, // Handle circular references
            WriteIndented = true // Optional: Makes the output more readable
        };

        // Return all jobs
        [HttpGet]
        public async Task<ActionResult<List<Job>>> GetAllJobs()
        {
            var jobs = await _context.Jobs
                .Include(job => job.Customer)
                .Include(job => job.JobStatus)
                .Include(job => job.Vehicle)
                .Include(job => job.JobDetails)
                .ToListAsync();
            if (jobs == null || jobs.Count == 0)
            {
                return NotFound("Job list not found");
            }
            else
            {
                var jsonResponse = jobs.Select(job => new
                {
                    JobId = job.JobId,
                    JobSummary = job.JobSummary,
                    JobCreatedAt = job.CreatedAt,
                    JobModifiedAt = job.ModifiedAt,
                    Customer = new
                    {
                        CustomerId = job.Customer.CustomerId,
                        CustomerName = job.Customer.Name,
                    },
                    Vehicle = new
                    {
                        VehicleId = job.Vehicle.VehicleId,
                        VehicleModel = job.Vehicle.Model,
                        VehicleBrand = job.Vehicle.Brand,
                        VehicleMilleage = job.Vehicle.Milleage
                    },
                    TotalJobDetails = job.JobDetails.Count
                });
                return Ok(JsonSerializer.Serialize(jsonResponse, _jsonSerializerOptions));
            }

        }

        // Get a specific job by ID
        [HttpGet("{job_id}")]
        public IActionResult GetJob(int job_id)
        {
            var job = _context.Jobs
                .Include(job => job.Customer)
                .Include(job => job.Vehicle)
                .Include(job => job.JobDetails)
                .FirstOrDefault(job => job.JobId == job_id);
            if (job == null)
            {
                return NotFound($"Job with id {job_id} not found");
            }
            else {
                var jsonResponse = new
                {
                    JobId = job.JobId,
                    JobSummary = job.JobSummary,
                    JobCreatedAt = job.CreatedAt,
                    JobModifiedAt = job.ModifiedAt,
                    Customer = new
                    {
                        CustomerId = job.Customer.CustomerId,
                        CustomerName = job.Customer.Name,
                    },
                    Vehicle = new
                    {
                        VehicleId = job.Vehicle.VehicleId,
                        VehicleModel = job.Vehicle.Model,
                        VehicleBrand = job.Vehicle.Brand,
                        VehicleMilleage = job.Vehicle.Milleage
                    },
                    NumberOfJobDetails = job.JobDetails.Count
                };
                return Ok(JsonSerializer.Serialize(jsonResponse, _jsonSerializerOptions));
            }
        }

        // Create a new job
        [HttpPost]
        public IActionResult CreateJob([FromBody] Job new_job)
        {
            Job job = new Job
            {
                JobSummary = new_job.JobSummary,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
                CustomerId = new_job.CustomerId,
                VehicleId = new_job.VehicleId,
                JobStatusId = new_job.JobStatusId
            };
            _context.Jobs.Add(job);

            if (_context.SaveChanges() > 0)
            {
                return CreatedAtAction(nameof(GetJob), new { job_id = job.JobId }, job);
            }
            else
            {
                return BadRequest("Job not created. Something went wrong.");
            }
        }

        // Update a job
        [HttpPatch("{job_id}")]
        public IActionResult UpdateJob(int job_id, [FromBody] Job updated_job)
        {
            var job = _context.Jobs.Find(job_id);
            if (job == null)
            {
                return NotFound($"Job with id {job_id} not found");
            }
            else
            {
                job.JobSummary = updated_job.JobSummary;
                job.ModifiedAt = DateTime.Now;
                job.CustomerId = updated_job.CustomerId;
                job.VehicleId = updated_job.VehicleId;
                job.JobStatusId = updated_job.JobStatusId;
                _context.Jobs.Update(job);

                if (_context.SaveChanges() > 0)
                {
                    return Ok($"Job {job.JobId} updated successfully");
                }
                else
                {
                    return BadRequest($"Job {job.JobId} not updated. Something went wrong.");
                }
            }
        }

        // Delete a job
        [HttpDelete("{job_id}")]
        public IActionResult DeleteJob(int job_id)
        {
            var job = _context.Jobs.Find(job_id);
            if (job == null)
            {
                return NotFound($"Job with id {job_id} not found");
            }
            else
            {
                _context.Jobs.Remove(job);
                if (_context.SaveChanges() > 0)
                {
                    return Ok($"Job {job.JobId} deleted successfully");
                }
                else
                {
                    return BadRequest($"Job {job.JobId} not deleted. Something went wrong.");
                }
            }
        }

        [HttpGet("{job_id}/jobdetails")]
        public IActionResult GetJobDetails(int job_id)
        {
            // Use eager loading to include JobDetails
            var job = _context.Jobs
                .Include(j => j.JobDetails) // Eagerly load JobDetails for the job
                .ThenInclude(jd => jd.JobStatus) //eagerly load JobStatuses for each JobDetail
                .FirstOrDefault(j => j.JobId == job_id); // Find the job by job_id

            if (job == null)
            {
                return NotFound($"Job with ID {job_id} not found.");
            }
            else
            {
                // Project the job and its details into a simplified structure
                var jsonResponse = new
                {
                    Job = new {
                        job.JobId,
                        job.JobSummary,
                        job.CreatedAt,
                        job.ModifiedAt,
                        job.CustomerId,
                        job.VehicleId,
                        job.JobStatusId,
                    },
                    TotalJobDetails = job.JobDetails.Count,
                    JobDetails = job.JobDetails.Select(detail => new
                    {
                        JobDetailId = detail.JobId,
                        JobDetailTitle = detail.Title,
                        JobDetailDescription = detail.Description,
                        JobDetailStatus = detail.JobStatus.JobStatusTitle
                    })
                };

                // Return the structured object
                return Ok(JsonSerializer.Serialize(jsonResponse, _jsonSerializerOptions));
            }

        }

    }
}
