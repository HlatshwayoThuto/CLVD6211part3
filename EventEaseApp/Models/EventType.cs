using System.ComponentModel.DataAnnotations;

namespace EventEaseApp.Models
{
    public class EventType
    {
        [Key]
        public int EventTypeID { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; }
    }
}