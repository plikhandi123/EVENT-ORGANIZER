using System;
using System.ComponentModel.DataAnnotations;

namespace EVENT_ORGANIZER.Models
{
    public class Events
    {
        public Events()
        {
            EventList = new List<Events>();
        }
        public List<Events> EventList { get; set; }
        public int EventId { get; set; }

        [Required(ErrorMessage = "EventName is required.")]
        [MaxLength(50, ErrorMessage = "EventName cannot exceed 50 characters.")]
        public string? EventName { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string? Description { get; set; }


        public DateTime? Eventdate { get; set; }

        
    }
}
