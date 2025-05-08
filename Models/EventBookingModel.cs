using System;
using System.ComponentModel.DataAnnotations;

namespace EVENT_ORGANIZER.Models
{
    public class EventBookingModel
    {
        public int? BookingId { get; set; }

        [Required(ErrorMessage = "Event name is required")]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Venues are required")]
        public string Venues { get; set; }

        [Required(ErrorMessage = "Services are required")]
        public string Services { get; set; }

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Event date is required")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string? Description { get; set; }

        
        public string? BookingStatus { get; set; }

        public int? UserId { get; set; }

        public List<VenueModel> AvailableVenues { get; set; }
        public List<BookedVenueModel> BookedVenues { get; set; }

    }
}
