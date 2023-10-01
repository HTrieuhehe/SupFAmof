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
        //public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "AccountIdBanned must be a positive integer.")]
        public int AccountIdBanned { get; set; }

        //public int BannedPersonId { get; set; }

       
        //public DateTime DayStart { get; set; }

        [Required]
        public DateTime DayEnd { get; set; }

        [Required]
        [MaxLength(256, ErrorMessage = "Note cannot exceed 256 characters.")]
        public string? Note { get; set; }

        //public bool IsActive { get; set; }
    }

    public class UpdateAccountBannedRequest
    {
        //public int Id { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "AccountIdBanned must be a positive integer.")]
        public int AccountIdBanned { get; set; }
        //public int BannedPersonId { get; set; }
        //public DateTime DayStart { get; set; }
        // public DateTime DayEnd { get; set; }

        [Required]
        [MaxLength(256, ErrorMessage = "Note cannot exceed 256 characters.")]
        public bool Note { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
