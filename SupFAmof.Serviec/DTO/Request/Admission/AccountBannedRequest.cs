using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ServiceStack.LicenseUtils;

namespace SupFAmof.Service.DTO.Request.Admission
{
    public class CreateAccountBannedRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "AccountIdBanned must be a positive integer.")]
        public int AccountIdBanned { get; set; }

        [Required]
        [DataType(DataType.DateTime, ErrorMessage = "DayEnd must be a valid DateTime.")]
        public DateTime DayEnd { get; set; }

        [Required]
        [MaxLength(256, ErrorMessage = "Note cannot exceed 256 characters.")]
        public string? Note { get; set; }
    }

    public class UpdateAccountBannedRequest
    {
        [Required]
        [MaxLength(256, ErrorMessage = "Note cannot exceed 256 characters.")]
        public string? Note { get; set; }

        [Required]
        //[Range(typeof(bool), "true", "false", ErrorMessage = "IsActive must be either true or false.")]
        public bool IsActive { get; set; }
    }
}
