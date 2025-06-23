using Microsoft.EntityFrameworkCore;

namespace EventEaseApp.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Venue> Venue { get; set; }
        public DbSet<Events> Events { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<EventType> EventType { get; set; }
    }
}
