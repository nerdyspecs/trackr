using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace trackr_api.Model
{
    public class User
    {
        [Key] // Mark UserId as the Primary Key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Automatically generate UserId (auto-increment)
        public int UserId { get; set; } // Primary Key
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Default 
        public DateTime ModifiedAt { get; set; } = DateTime.Now; // Default

        //foreignkey
        public int? RoleId { get; set; } // Mechanic, FrontDesk, SpareParts, Management, Admin, Customer
        public Role Role { get; set; }

    }
}
