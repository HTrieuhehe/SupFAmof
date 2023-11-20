using System.ComponentModel.DataAnnotations;

namespace SupFAmof.Service.DTO.Request
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
        
        //public string? FcmToken { get; set; }
    }
}
