using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace trackr_api.Model
{
    public class Vehicle
    {
        [Key] // Mark UserId as the Primary Key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Automatically generate VehicleId (auto-increment)
        public int VehicleId { get; set; } // Primary Key
        public string RegistrationNumber { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public int Milleage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Default 
        public DateTime ModifiedAt {  get; set; } = DateTime.Now; // Default

        // foreignkey
        public int? CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
