using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EventsManagementInterface.Data.Models.Vendor;
using EventsManagementInterface.Data.Models.Administration;
using EventsManagementInterface.Data.Models.Attendee;

namespace EventsManagementInterface.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer();
        }

        public virtual DbSet<Attendee> Attendee { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<VendorInput> VendorInput { get; set; }
        public virtual DbSet<Order> Order { get; set; }
    }
}