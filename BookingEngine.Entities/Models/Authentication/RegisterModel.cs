using System.ComponentModel.DataAnnotations;

namespace BookingEngine.Entities.Models.Authentication
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Username is required!")]
        public string Username { get; set; }

        [Required(ErrorMessage = "E-mail is required!")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required!")]
        public string Password { get; set; }
    }
}
