using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using trackr_api.Data;


namespace trackr_api.Model
{
    public class Customer
    {
        [Key] // Mark CustomerId as the Primary Key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Automatically generate CustomerId (auto-increment)
        public int CustomerId { get; set; } // Primary Key
        public string Name { get; set; }
        public string PicContact { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Default 
        public DateTime ModifiedAt { get; set; } = DateTime.Now; // Default

        // foreignkey
        public int? UserId { get; set; }
        public User User { get; set; }

        // Navigation Property
        public virtual ICollection<Vehicle> Vehicles { get; set; }


    }
}
