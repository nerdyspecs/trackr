using Microsoft.EntityFrameworkCore;
using trackr_api.Model;

namespace trackr_api.Data
{
    public class TrackrDbContext : DbContext
    {
        public TrackrDbContext(DbContextOptions<TrackrDbContext> options) : base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
    }
}
