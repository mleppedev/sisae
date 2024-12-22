using System;
using System.ComponentModel.DataAnnotations;

namespace sisae.Models
{
    public class EventLog
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string EventType { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime EventDate { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; } // Assuming the event is linked to a user
    }
}
