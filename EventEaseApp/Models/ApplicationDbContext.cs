using Microsoft.EntityFrameworkCore;

namespace EventEaseApp.Models
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) 
        {
            
        }
        public DbSet<Venue> Venue { get; set; }
        public DbSet<Events> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<EventType> EventType { get; set; }

    }
}
