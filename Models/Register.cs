using System.ComponentModel.DataAnnotations;

namespace EVENT_ORGANIZER.Models
{
    public class Register
    {
        public Register()
        {
            RegisterList = new List<Register>();
        }
        public List<Register> RegisterList { get; set; }
        public int? UserId { get; set; }
        [Required(ErrorMessage = "Please enter the fist name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Please enter the fist name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Please enter the Mobile")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid mobile number. It should be 10 digits.")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Please enter the Email")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter the Address")]
        public string Address { get; set; }
        
        [Required(ErrorMessage = "Please enter the password")]
        public string Password { get; set; }
        public string? UserType { get;  set; }
        
        public string?  Status { get; set; }

    }
}