using System;
using System.ComponentModel.DataAnnotations;

namespace EVENT_ORGANIZER.Models
{
    public class Booking
    {
        public int BookingId { get; set; } // Auto-incremented Primary Key

       
        public int UserId { get; set; }

        [Required(ErrorMessage = "Event Name is required.")]
        [StringLength(50, ErrorMessage = "Event Name cannot exceed 50 characters.")]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(50, ErrorMessage = "Location cannot exceed 50 characters.")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Venue is required.")]
        [StringLength(50, ErrorMessage = "Venue cannot exceed 50 characters.")]
        public string Venues { get; set; }

        [Required(ErrorMessage = "Services information is required.")]
        [StringLength(50, ErrorMessage = "Services cannot exceed 50 characters.")]
        public string Services { get; set; }

        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be a positive number.")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Event Date is required.")]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Booking Status is required.")]
        [StringLength(50)]
        public string BookingStatus { get; set; } = "Pending"; // Default value
    }
}
