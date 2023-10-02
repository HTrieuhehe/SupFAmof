using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ServiceStack.LicenseUtils;

namespace SupFAmof.Service.DTO.Request.Staff
{
    public class UpdateStaffRequest
    {
        [MinLength(6,
        ErrorMessage = "Value for {0} must longer than {1} characters.")]
        [MaxLength(30,
        ErrorMessage = "Value for {0} must shorter than {1} characters.")]
        [Required]
        public string? Username { get; set; }

        [MinLength(6,
        ErrorMessage = "Value for {0} must longer than {1} characters.")]
        [MaxLength(10,
        ErrorMessage = "Value for {0} must shorter than {1} characters.")]
        [Required]
        public string? Password { get; set; }

        [Range(typeof(bool), "true", "false", ErrorMessage = "IsAvailable must be either true or false.")]
        public bool IsAvailable { get; set; }
    }
}
