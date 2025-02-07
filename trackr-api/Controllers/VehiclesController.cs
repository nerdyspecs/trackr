using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using trackr_api.Model;
using trackr_api.Data;
using trackr_api.Filters;

namespace trackr_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiclesController : BaseController
    {
        private readonly TrackrDbContext _context;
        public VehiclesController(TrackrDbContext context, ILogger<VehiclesController> logger) : base(logger)
        {
            _context = context;
        }

        // Return all vehicles (admin)
        [HttpGet]
        [ServiceFilter(typeof(AuthFilter))]
        public async Task<IActionResult> GetAllVehicles()
        {
            try
            {
                var vehicles = await _context.Vehicles
                                .Include(a => a.Customer)         // Include Customer related to the vehicle
                                .Include(a => a.Jobs)             // Include Jobs for the vehicle
                                    .ThenInclude(job => job.JobStatus)        // Include JobStatus for each Job
                                .Include(vehicle => vehicle.Jobs)         // Include JobDetails for each Job
                                    .ThenInclude(job => job.JobDetails)       // Include JobDetails for each Job
                                    .ThenInclude(jobDetail => jobDetail.JobStatus) // Include JobStatus for JobDetails
                                .ToListAsync();

                if (vehicles == null)
                {
                    return NotFound("Vehicle list not found");
                }
                else
                {

                    var jsonResponse = vehicles.Select(vehicle => new
                    {
                        VehicleId = vehicle.VehicleId,
                        VehicleModel = vehicle.Model,
                        VehicleBrand = vehicle.Brand,
                        VehicleMileage = vehicle.Milleage,
                        VehicleCustomer = new
                        {
                            CustomerId = vehicle.Customer.CustomerId,
                            CustomerName = vehicle.Customer.Name,
                            CustomerPicContact = vehicle.Customer.PicContact,
                            CustomerAddress = vehicle.Customer.Address,
                            CustomerCreatedAt = vehicle.Customer.CreatedAt
                        },
                        Jobs = vehicle.Jobs.Select(job => new
                        {
                            JobId = job.JobId,
                            JobSummary = job.JobSummary,
                            JobStatus = job.JobStatus.JobStatusTitle,
                            TotalJobDetail = job.JobDetails.Count
                        })
                    });
                    return Ok(JsonSerializer.Serialize(jsonResponse, _jsonSerializerOptions));
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
            
        }

        [HttpGet("{vehicle_id}")]
        [ServiceFilter(typeof(AuthFilter))]
        public IActionResult GetVehicle(int vehicle_id)
        {
            try
            {
                var vehicle = _context.Vehicles
               .Include(vehicle => vehicle.Customer)         // Include Customer related to the vehicle
               .Include(vehicle => vehicle.Jobs)             // Include Jobs for the vehicle
                   .ThenInclude(job => job.JobStatus)        // Include JobStatus for each Job
               .Include(vehicle => vehicle.Jobs)         // Include JobDetails for each Job
                   .ThenInclude(job => job.JobDetails)       // Include JobDetails for each Job
                   .ThenInclude(jobDetail => jobDetail.JobStatus) // Include JobStatus for JobDetails
               .FirstOrDefault(vehicle => vehicle.VehicleId == vehicle_id); // Fetch the vehicle by ID

                if (vehicle == null)
                {
                    return NotFound();
                }
                else
                {
                    var jsonResponse = new
                    {
                        VehicleId = vehicle.VehicleId,
                        VehicleModel = vehicle.Model,
                        VehicleBrand = vehicle.Brand,
                        VehicleMileage = vehicle.Milleage,
                        VehicleCustomer = new
                        {
                            CustomerId = vehicle.Customer.CustomerId,
                            CustomerName = vehicle.Customer.Name,
                            CustomerPicContact = vehicle.Customer.PicContact,
                            CustomerAddress = vehicle.Customer.Address,
                            CustomerCreatedAt = vehicle.Customer.CreatedAt
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

        [HttpPost]
        [ServiceFilter(typeof(AuthFilter))]
        public IActionResult CreateVehicle([FromBody] Vehicle new_vehicle)
        {
            try
            {
                Vehicle vehicle = new Vehicle
                {
                    RegistrationNumber = new_vehicle.RegistrationNumber,
                    Model = new_vehicle.Model,
                    Brand = new_vehicle.Brand,
                    Milleage = new_vehicle.Milleage,
                    CustomerId = new_vehicle.CustomerId
                };
                _context.Vehicles.Add(vehicle); // Highlighted change: added `vehicle` instead of `new_vehicle`

                if (_context.SaveChanges() > 0)
                {
                    return CreatedAtAction(nameof(GetVehicle), new { id = vehicle.VehicleId }, vehicle);
                }
                else
                {
                    return BadRequest("Vehicle not created. Something went wrong.");
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
           
        }

        [HttpPatch("{vehicle_id}")]
        [ServiceFilter(typeof(AuthFilter))]
        public IActionResult UpdateVehicle(int vehicle_id, [FromBody] Vehicle updated_vehicle)
        {
            try
            {
                var vehicle = _context.Vehicles.Find(vehicle_id);
                if (vehicle == null)
                {
                    return NotFound($"Vehicle with id {vehicle_id} not found"); // Highlighted change: fixed typo and adjusted message
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
            catch (Exception ex)
            {
                return HandleError(ex);
            }
           
        }

        [HttpDelete("{vehicle_id}")]
        [ServiceFilter(typeof (AuthFilter))]    
        public IActionResult DeleteVehicle(int vehicle_id)
        {
            try
            {
                var vehicle = _context.Vehicles.Find(vehicle_id);
                if (vehicle == null)
                {
                    return NotFound($"Vehicle with id {vehicle_id} not found");
                }
                else
                {
                    _context.Vehicles.Remove(vehicle);
                    if (_context.SaveChanges() > 0)
                    {
                        return Ok($"Vehicle Deleted: {vehicle.RegistrationNumber}"); // Highlighted change: included vehicle's registration number
                    }
                    else
                    {
                        return BadRequest($"Vehicle {vehicle.VehicleId} not deleted. Something went wrong");
                    }
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        // Get jobs and job details for vehicles
        [HttpGet("{vehicle_id}/jobsandDetails")]
        [ServiceFilter(typeof(AuthFilter))]
        public IActionResult GetJobAndDetails(int vehicle_id)
        {
            try
            {
                var vehicle = _context.Vehicles
           .Include(vehicle => vehicle.Customer)         // Include Customer related to the vehicle
           .Include(vehicle => vehicle.Jobs)             // Include Jobs for the vehicle
               .ThenInclude(job => job.JobStatus)        // Include JobStatus for each Job
           .Include(a => a.Jobs)         // Include JobDetails for each Job
               .ThenInclude(job => job.JobDetails)       // Include JobDetails for each Job
               .ThenInclude(jobDetail => jobDetail.JobStatus) // Include JobStatus for JobDetails
           .FirstOrDefault(vehicle => vehicle.VehicleId == vehicle_id); // Fetch the vehicle by ID

                if (vehicle == null)
                {
                    return NotFound($"Vehicle with id {vehicle_id} not found.");
                }

                // Flatten the response structure
                var jsonResponse = new
                {
                    VehicleId = vehicle.VehicleId,
                    VehicleModel = vehicle.Model,
                    VehicleBrand = vehicle.Brand,
                    VehicleMileage = vehicle.Milleage,
                    Jobs = vehicle.Jobs.Select(job => new
                    {
                        JobId = job.JobId,
                        JobSummary = job.JobSummary,
                        JobStatus = job.JobStatus.JobStatusTitle,
                        JobDetails = job.JobDetails.Select(detail => new
                        {
                            JobDetailId = detail.JobDetailId,
                            JobDetailTitle = detail.Title,
                            JobDetailDescription = detail.Description,
                            JobDetailCreatedAt = detail.CreatedAt,
                            JobDetailModifiedAt = detail.ModifiedAt
                        })
                    })
                };
                return Ok(JsonSerializer.Serialize(jsonResponse, _jsonSerializerOptions));
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [HttpGet("{vehicle_id}/jobs")]
        [ServiceFilter(typeof(AuthFilter))]
        public IActionResult GetJob(int vehicle_id)
        {
            try
            {
                var vehicle = _context.Vehicles
                              .Include(vehicle => vehicle.Customer)         // Include Customer related to the vehicle
                              .Include(vehicle => vehicle.Jobs)             // Include Jobs for the vehicle
                                  .ThenInclude(job => job.JobStatus)        // Include JobStatus for each Job
                            .FirstOrDefault(vehicle => vehicle.VehicleId == vehicle_id); // Fetch the vehicle by ID

                if (vehicle == null)
                {
                    return NotFound($"Vehicle with id {vehicle_id} not found.");
                }
                else
                {
                    var jsonResponse = new
                    {
                        VehicleId = vehicle.VehicleId,
                        VehicleModel = vehicle.Model,
                        VehicleBrand = vehicle.Brand,
                        VehicleMileage = vehicle.Milleage,
                        Jobs = vehicle.Jobs.Select(job => new
                        {
                            JobId = job.JobId,
                            JobSummary = job.JobSummary,
                            JobStatus = job.JobStatus.JobStatusTitle
                        })
                    };
                    return Ok(JsonSerializer.Serialize(jsonResponse, _jsonSerializerOptions));
                }
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
    }
}
