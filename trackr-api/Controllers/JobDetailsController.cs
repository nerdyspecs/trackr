using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using trackr_api.Model;
using trackr_api.Data;

namespace trackr_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobDetailController : Controller
    {
        private readonly TrackrDbContext _context;

        public JobDetailController(TrackrDbContext context)
        {
            _context = context;
        }

        // Return all job details
        [HttpGet]
        public async Task<ActionResult<List<JobDetail>>> GetAllJobDetails()
        {
            var jobDetails = await _context.JobDetails.ToListAsync();
            if (jobDetails == null)
            {
                return NotFound("Job details list not found");
            }
            else
            {
                return Ok(jobDetails);
            }
        }

        // Return a specific job detail by ID
        [HttpGet("{job_detail_id}")]
        public IActionResult GetJobDetail(int job_detail_id)
        {
            var jobDetail = _context.JobDetails.Find(job_detail_id);
            if (jobDetail == null)
            {
                return NotFound($"Job detail with ID {job_detail_id} not found");
            }
            else
            {
                return Ok(jobDetail);
            }
        }

        // Create a new job detail
        [HttpPost]
        public IActionResult CreateJobDetail([FromBody] JobDetail new_job_detail)
        {
            JobDetail jobDetail = new JobDetail
            {
                Title = new_job_detail.Title,
                Description = new_job_detail.Description,
                JobId = new_job_detail.JobId,
                JobStatusId = new_job_detail.JobStatusId
            };
            _context.JobDetails.Add(new_job_detail);

            if (_context.SaveChanges() > 0)
            {
                return CreatedAtAction(nameof(GetJobDetail), new { job_detail_id = jobDetail.JobDetailId }, jobDetail);
            }
            else
            {
                return BadRequest("Job detail not created. Something went wrong.");
            }
        }

        // Update a job detail
        [HttpPatch("{job_detail_id}")]
        public IActionResult UpdateJobDetail(int job_detail_id, [FromBody] JobDetail updated_job_detail)
        {
            var jobDetail = _context.JobDetails.Find(job_detail_id);
            if (jobDetail == null)
            {
                return NotFound($"Job detail with ID {job_detail_id} not found");
            }
            else
            {
                jobDetail.Title = updated_job_detail.Title;
                jobDetail.Description = updated_job_detail.Description;
                jobDetail.JobId = updated_job_detail.JobId;
                jobDetail.JobStatusId = updated_job_detail.JobStatusId;
                jobDetail.ModifiedAt = DateTime.Now;

                _context.JobDetails.Update(jobDetail);
                if (_context.SaveChanges() > 0)
                {
                    return Ok($"Job detail {jobDetail.JobDetailId} updated successfully");
                }
                else
                {
                    return BadRequest($"Job detail {jobDetail.JobDetailId} not updated. Something went wrong.");
                }
            }
        }

        // Delete a job detail
        [HttpDelete("{job_detail_id}")]
        public IActionResult DeleteJobDetail(int job_detail_id)
        {
            var jobDetail = _context.JobDetails.Find(job_detail_id);
            if (jobDetail == null)
            {
                return NotFound($"Job detail {job_detail_id} not found");
            }
            else
            {
                _context.JobDetails.Remove(jobDetail);
                if (_context.SaveChanges() > 0)
                {
                    return Ok($"Job detail {jobDetail.JobDetailId} deleted successfully");
                }
                else
                {
                    return BadRequest($"Job detail {jobDetail.JobDetailId} not deleted. Something went wrong.");
                }
            }
        }
    }
}
