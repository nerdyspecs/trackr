using Microsoft.AspNetCore.Mvc;
using trackr_api.Model;
using trackr_api.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using trackr_api.Filters;

namespace trackr_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobStatusController : BaseController
    {
        private readonly TrackrDbContext _context;
        public JobStatusController(TrackrDbContext context, ILogger<JobStatusController> logger) : base (logger)
        {
            _context = context;
        }

        // Return all job statuses
        [HttpGet]
        [ServiceFilter(typeof(AuthFilter))]
        public async Task <IActionResult> GetAllJobStatuses()
        {
            try
            {
                var jobstatuses = await _context.JobStatuses.ToListAsync();
                if (jobstatuses == null || jobstatuses.Count == 0)
                {
                    return NotFound("Job status list not found");
                }
                else
                {
                    var jsonResponse = jobstatuses.Select(jobstatus => new
                    {
                        JobStatusId = jobstatus.JobStatusId,
                        JobStatusTitle = jobstatus.JobStatusTitle,
                        JobStatusDescription = jobstatus.JobStatusDescription,
                        JobStatusCreateAt = jobstatus.CreatedAt,
                        JobStatusModifiedAt = jobstatus.ModifiedAt
                    });

                    return Ok(JsonSerializer.Serialize(jsonResponse, _jsonSerializerOptions));
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
           
        }

        // Get a specific job status by ID
        [HttpGet("{job_status_id}")]
        [ServiceFilter(typeof(AuthFilter))] 
        public IActionResult GetJobStatus(int job_status_id)
        {
            try
            {
                var jobstatus = _context.JobStatuses.Find(job_status_id);
                if (jobstatus == null)
                {
                    return NotFound($"Job status with id {job_status_id} not found");
                }
                else
                {
                    var jsonResponse = new
                    {
                        JobStatusId = jobstatus.JobStatusId,
                        JobStatusTitle = jobstatus.JobStatusTitle,
                        JobStatusDescription = jobstatus.JobStatusDescription,
                        JobStatusCreateAt = jobstatus.CreatedAt,
                        JobStatusModifiedAt = jobstatus.ModifiedAt
                    };

                    return Ok(JsonSerializer.Serialize(jsonResponse, _jsonSerializerOptions));
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
         
        }

        // Create a new job status
        [HttpPost]
        [ServiceFilter(typeof(AuthFilter))]
        public IActionResult CreateJobStatus([FromBody] JobStatus new_job_status)
        {
            try
            {
                JobStatus jobStatus = new JobStatus
                {
                    JobStatusTitle = new_job_status.JobStatusTitle,
                    JobStatusDescription = new_job_status.JobStatusDescription,
                    CreatedAt = DateTime.Now,
                    ModifiedAt = DateTime.Now
                };
                _context.JobStatuses.Add(jobStatus);

                if (_context.SaveChanges() > 0)
                {
                    return CreatedAtAction(nameof(GetJobStatus), new { job_status_id = jobStatus.JobStatusId }, jobStatus);
                }
                else
                {
                    return BadRequest("Job status not created. Something went wrong.");
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
           
        }

        // Update a job status
        [HttpPatch("{job_status_id}")]
        public IActionResult UpdateJobStatus(int job_status_id, [FromBody] JobStatus updated_job_status)
        {
            var jobStatus = _context.JobStatuses.Find(job_status_id);
            if (jobStatus == null)
            {
                return NotFound($"Job status with id {job_status_id} not found");
            }
            else
            {
                jobStatus.JobStatusTitle = updated_job_status.JobStatusTitle;
                jobStatus.JobStatusDescription = updated_job_status.JobStatusDescription;
                jobStatus.ModifiedAt = DateTime.Now;
                _context.JobStatuses.Update(jobStatus);

                if (_context.SaveChanges() > 0)
                {
                    return Ok($"Job status {jobStatus.JobStatusId} updated successfully");
                }
                else
                {
                    return BadRequest($"Job status {jobStatus.JobStatusId} not updated. Something went wrong.");
                }
            }
        }

        // Delete a job status
        [HttpDelete("{job_status_id}")]
        [ServiceFilter(typeof(AuthFilter))]
        public IActionResult DeleteJobStatus(int job_status_id)
        {
            try
            {
                var jobStatus = _context.JobStatuses.Find(job_status_id);
                if (jobStatus == null)
                {
                    return NotFound($"Job status with id {job_status_id} not found");
                }
                else
                {
                    _context.JobStatuses.Remove(jobStatus);
                    if (_context.SaveChanges() > 0)
                    {
                        return Ok($"Job status {jobStatus.JobStatusId} deleted successfully");
                    }
                    else
                    {
                        return BadRequest($"Job status {jobStatus.JobStatusId} not deleted. Something went wrong.");
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
