using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using trackr_api.Model;
using trackr_api.Data;

namespace trackr_api.Services
{
    public class PopulateData  // Changed from static class to non-static class
    {
        private readonly TrackrDbContext _context;  // Kept _context as instance field

        public PopulateData(TrackrDbContext context)  // Constructor to inject context
        {
            _context = context;
        }

        public void SeedData()  // Changed from static method to non-static method
        {
            // Check if data already exists to avoid re-seeding on every run
            if (_context.Customers.Any()) return;

            // Seed Roles
            var roles = new List<Role>
            {
                new Role { RoleName = "Admin", RoleDescription = "Administrator with full access" },
                new Role { RoleName = "Mechanic", RoleDescription = "Mechanic with access to job and vehicle details" },
                new Role { RoleName = "FrontDesk", RoleDescription = "Handles customer inquiries and bookings" },
                new Role { RoleName = "Management", RoleDescription = "Management with overview of operations" }
            };

            _context.Roles.AddRange(roles);  // Changed to _context
            _context.SaveChanges();  // Changed to _context

            // Seed Users
            var users = new List<User>
            {
                new User { Username = "admin", PasswordHash = "hashedpassword1", RoleId = 1 },
                new User { Username = "mechanic1", PasswordHash = "hashedpassword2", RoleId = 2 },
                new User { Username = "frontdesk1", PasswordHash = "hashedpassword3", RoleId = 3 },
                new User { Username = "manager1", PasswordHash = "hashedpassword4", RoleId = 4 }
            };

            _context.Users.AddRange(users);  // Changed to _context
            _context.SaveChanges();  // Changed to _context

            // Seed Customers
            var customers = new List<Customer>
            {
                new Customer { Name = "Customer A", PicContact = "John Doe", Address = "123 Street A", UserId = 1 },
                new Customer { Name = "Customer B", PicContact = "Jane Smith", Address = "456 Street B", UserId = 2 },
                new Customer { Name = "Customer C", PicContact = "James Brown", Address = "789 Street C", UserId = 3 }
            };

            _context.Customers.AddRange(customers);  // Changed to _context
            _context.SaveChanges();  // Changed to _context

            // Seed Vehicles
            var vehicles = new List<Vehicle>();
            var random = new Random();

            foreach (var customer in customers)
            {
                var numOfVehicles = random.Next(1, 4); // 1 to 3 vehicles per customer
                for (int i = 0; i < numOfVehicles; i++)
                {
                    vehicles.Add(new Vehicle
                    {
                        RegistrationNumber = $"REG{random.Next(100, 999)}",
                        Model = $"Model {random.Next(1, 5)}",
                        Brand = $"Brand {random.Next(1, 5)}",
                        Milleage = random.Next(20000, 200000),
                        CustomerId = customer.CustomerId
                    });
                }
            }

            _context.Vehicles.AddRange(vehicles);  // Changed to _context
            _context.SaveChanges();  // Changed to _context

            // Seed Job Statuses
            var jobStatuses = new List<JobStatus>
            {
                new JobStatus { JobStatusTitle = "Pending", JobStatusDescription = "Job is awaiting assignment" },
                new JobStatus { JobStatusTitle = "In Progress", JobStatusDescription = "Job is currently being worked on" },
                new JobStatus { JobStatusTitle = "Completed", JobStatusDescription = "Job has been completed" },
                new JobStatus { JobStatusTitle = "Cancelled", JobStatusDescription = "Job has been cancelled" }
            };

            _context.JobStatuses.AddRange(jobStatuses);  // Changed to _context
            _context.SaveChanges();  // Changed to _context

            // Seed Jobs
            var jobs = new List<Job>();
            foreach (var vehicle in vehicles)
            {
                var numOfJobs = random.Next(5, 7); // 5 to 6 jobs per vehicle
                for (int i = 0; i < numOfJobs; i++)
                {
                    jobs.Add(new Job
                    {
                        JobSummary = $"Job Summary {random.Next(1, 10)}",
                        CustomerId = vehicle.CustomerId,
                        VehicleId = vehicle.VehicleId,
                        JobStatusId = random.Next(1, 5) // Randomly assign job status
                    });
                }
            }

            _context.Jobs.AddRange(jobs);  // Changed to _context
            _context.SaveChanges();  // Changed to _context

            // Seed Job Details
            var jobDetails = new List<JobDetail>();
            foreach (var job in jobs)
            {
                var numOfJobDetails = random.Next(3, 7); // 3 to 6 job details per job
                for (int i = 0; i < numOfJobDetails; i++)
                {
                    jobDetails.Add(new JobDetail
                    {
                        Title = $"Job Detail {random.Next(1, 10)}",
                        Description = $"Description for job detail {random.Next(1, 10)}",
                        JobId = job.JobId,
                        JobStatusId = random.Next(1, 5) // Randomly assign job status
                    });
                }
            }

            _context.JobDetails.AddRange(jobDetails);  // Changed to _context
            _context.SaveChanges();  // Changed to _context
        }
    }
}
