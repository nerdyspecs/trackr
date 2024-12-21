using Microsoft.AspNetCore.Mvc;
using trackr_api.Model;
using trackr_api.Data;

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

        // Return all jobs
        [HttpGet]
        public async Task<ActionResult<List<Job>>> GetAllJobs()
        {
            var jobs = _context.Jobs.ToList();
            if (jobs == null || jobs.Count == 0)
            {
                return NotFound("Job list not found");
            }
            else {
                return Ok(jobs);
            }
            
        }

        // Get a specific job by ID
        [HttpGet("{job_id}")]
        public IActionResult GetJob(int job_id)
        {
            var job = _context.Jobs.Find(job_id);
            if (job == null)
            {
                return NotFound($"Job with id {job_id} not found");
            }
            return Ok(job);
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
    }
}
