namespace EVENT_ORGANIZER.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Venues
    {
        public Venues()
        {
            VenueList = new List<Venues>();
        }
        public List<Venues> VenueList { get; set; }
        [Key]
        public int VenueId { get; set; }

        [Required(ErrorMessage = "VenueName is required")]
        [StringLength(50, ErrorMessage = "VenueName cannot exceed 50 characters")]
        public string VenueName { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }

        

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than zero")]
        public decimal Capacity { get; set; }

    }


}
