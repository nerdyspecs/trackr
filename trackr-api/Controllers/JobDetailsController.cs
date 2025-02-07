using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using trackr_api.Model;
using trackr_api.Data;
using System.Text.Json;
using trackr_api.Filters;

namespace trackr_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobDetailController : BaseController
    {
        private readonly TrackrDbContext _context;

        public JobDetailController(TrackrDbContext context, ILogger<JobDetailController> logger) : base(logger)
        {
            _context = context;
        }

        // Return all job details
        [HttpGet]
        [ServiceFilter(typeof(AuthFilter))]
        public async Task<IActionResult> GetAllJobDetails()
        {
            try
            {
                var jobDetails = await _context.JobDetails
               .Include(jobdetail => jobdetail.JobStatus)
               .ToListAsync();
                if (jobDetails == null)
                {
                    return NotFound("Job details list not found");
                }
                else
                {
                    var JsonResponse = jobDetails.Select(jobdetail => new
                    {
                        Job = new
                        {
                            JobId = jobdetail.JobId
                        },
                        JobDetailId = jobdetail.JobDetailId,
                        JobDetailTitle = jobdetail.Title,
                        JobDetailDescription = jobdetail.Description,
                        JobDetailJobStatus = jobdetail.JobStatus.JobStatusTitle,
                        JobDetailCreateAt = jobdetail.CreatedAt
                    });
                    return Ok(JsonSerializer.Serialize(JsonResponse, _jsonSerializerOptions));
                }
            }
            catch (Exception ex) { 
                return HandleError(ex);
            }
           
        }

        // Return a specific job detail by ID
        [HttpGet("{job_detail_id}")]
        [ServiceFilter(typeof(AuthFilter))]
        public IActionResult GetJobDetail(int job_detail_id)
        {
            try
            {
                var jobdetail = _context.JobDetails
                .Include(job => job.JobStatus)
                .Include(jobdetail => jobdetail.Job)
                .ThenInclude(job => job.JobStatus)
                .FirstOrDefault(jobdetail => jobdetail.JobDetailId == job_detail_id);
                if (jobdetail == null)
                {
                    return NotFound($"Job detail with ID {job_detail_id} not found");
                }
                else
                {

                    var JsonResponse = new
                    {
                        Job = new
                        {
                            JobId = jobdetail.JobId
                        },
                        JobDetailId = jobdetail.JobDetailId,
                        JobDetailTitle = jobdetail.Title,
                        JobDetailDescription = jobdetail.Description,
                        JobDetailJobStatus = jobdetail.JobStatus.JobStatusTitle,
                        JobDetailCreateAt = jobdetail.CreatedAt
                    };
                    return Ok(JsonSerializer.Serialize(JsonResponse, _jsonSerializerOptions));
                }
            }
            catch (Exception ex) {
                return HandleError(ex);
            }
        }

        // Create a new job detail
        [HttpPost]
        [ServiceFilter(typeof(AuthFilter))]
        public IActionResult CreateJobDetail([FromBody] JobDetail new_job_detail)
        {
            try
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
            catch (Exception ex) {
                return HandleError(ex);
            }
            
        }

        // Update a job detail
        [HttpPatch("{job_detail_id}")]
        [ServiceFilter(typeof(AuthFilter))]
        public IActionResult UpdateJobDetail(int job_detail_id, [FromBody] JobDetail updated_job_detail)
        {
            try
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
            catch (Exception ex)
            {
                return HandleError(ex);
            }
            
        }

        // Delete a job detail
        [HttpDelete("{job_detail_id}")]
        [ServiceFilter(typeof(AuthFilter))]
        public IActionResult DeleteJobDetail(int job_detail_id)
        {
            try
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
            catch (Exception ex)
            {
                return HandleError(ex);
            }
            
        }
    }
}
