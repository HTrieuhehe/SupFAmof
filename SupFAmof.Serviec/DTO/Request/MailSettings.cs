using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SupFAmof.Service.DTO.Request
{
    public class MailSettings
    {
        public string? Mail { get; set; }
        public string? DisplayName { get; set; }
        public string? Password { get; set; }
        public string? Host { get; set; }
        public int Port { get; set; }
    }

    public class MailPaths
    {
        public Dictionary<string, string>? Paths { get; set; }
    }

    public class MailVerificationRequest
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Subject { get; set; }

        public int Code { get; set; }
    }

    public class MailBookingRequest
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? RegistrationCode { get; set; }

        [Required]
        public string? PostName { get; set; }

        [Required]
        public string? DateFrom { get; set; }

        public string? DateTo { get; set; }

        [Required]
        public string? PositionName { get; set; }

        [Required]
        public string? TimeFrom { get; set; }

        public string? TimeTo { get; set; }

        [Required]
        public string? SchoolName { get; set; }

        [Required]
        public string? Location { get; set; }

        public string? Note { get; set; }
    }
}
