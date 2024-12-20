using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace trackr_api.Model
{
    public class JobStatus
    {
        [Key] // Mark UserId as the Primary Key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Automatically generate JobStatusId (auto-increment)
        public int JobStatusId { get; set; }
        public string JobStatusTitle { get; set; }
        public string JobStatusDescription { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Default 
        public DateTime ModifiedAt { get; set; } = DateTime.Now; // Default
    }
}
