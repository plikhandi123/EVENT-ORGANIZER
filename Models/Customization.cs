using System.ComponentModel.DataAnnotations;

namespace EVENT_ORGANIZER.Models
{
    public class Customization
    {
        [Key]
        public int CustomizationId { get; set; } // Primary Key

        [Required]
        public int EventId { get; set; } // Foreign Key (Event Table)

     
        [StringLength(50)]
        public string? Theme { get; set; }
        public string? SpecialRequests { get; set; }

    }
}
