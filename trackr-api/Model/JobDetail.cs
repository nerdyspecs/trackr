using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace trackr_api.Model
{
    public class JobDetail
    {
        [Key] // Mark JobDetailId as the Primary Key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Automatically generate JobDetailId (auto-increment)
        public int JobDetailId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Default 
        public DateTime ModifiedAt { get; set; } = DateTime.Now; // Default

        //foreignkey
        public int? JobId { get; set; }
        public Job Job { get; set; }
        public int? JobStatusId { get; set; }
        public JobStatus JobStatus { get; set; }

    }

}
