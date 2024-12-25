using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace trackr_api.Model
{
    public class Job
    {
        [Key] // Mark JobId as the Primary Key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Automatically generate JobId (auto-increment)
        public int JobId { get; set; } // Primary Key
        public string JobSummary { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Default 
        public DateTime ModifiedAt { get; set; } = DateTime.Now; // Default

        //foreignkey
        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int? VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
        public int? JobStatusId { get; set; }
        public JobStatus JobStatus { get; set; }


        // Navigation Property
        public virtual ICollection<JobDetail> JobDetails{ get; set; }
    }
}
