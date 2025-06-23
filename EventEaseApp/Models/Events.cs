using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventEaseApp.Models
{
    public class Events
    {
        [Key]
        public int EventID { get; set; }

        [Required]
        [StringLength(250)]
        public string EventName { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [StringLength(250)]
        public string Descriptions { get; set; }

        [ForeignKey("Venue")] 
        public int? VenueID { get; set; }
        public Venue? Venue { get; set; }

        public int? EventTypeID { get; set; }//STEP 4
        public EventType? EventType { get; set; }
    }
}

