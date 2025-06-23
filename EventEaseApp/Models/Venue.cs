using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEaseApp.Models
{
    public class Venue
    {
        [Key]
        public int VenueID { get; set; }

        [Required]
        [StringLength(250)]
        public string VenueName { get; set; }

        [Required]
        [StringLength(250)]
        public string Locations { get; set; }

        public int Capacity { get; set; }


        //this is to store the url of the image uploaded
        public String? ImageUrl { get; set; }

        //add this new one ont to upload from the create/ edit form
        [NotMapped]

        public IFormFile? ImageFile { get; set; }
    }
}
