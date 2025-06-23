using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventEaseApp.Models
{
    public class Booking
    {
        [Key]
        public int BookingID { get; set; }

        [ForeignKey("Venue")]
        public int? VenueID { get; set; }
        public Venue? Venue { get; set; }

        [ForeignKey("Events")]
        public int? EventID { get; set; }
        public Events? Events { get; set; }
        public DateTime BookingDate { get; set; }
    }
}
