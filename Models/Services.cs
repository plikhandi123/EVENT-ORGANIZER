using System.ComponentModel.DataAnnotations;

namespace EVENT_ORGANIZER.Models
{
    public class Services
    {
        [Key]
        public int ServiceId { get; set; }

        [Required]
        [StringLength(50)]
        public string ServiceName { get; set; }

        
        public string Description { get; set; }
    }
}
