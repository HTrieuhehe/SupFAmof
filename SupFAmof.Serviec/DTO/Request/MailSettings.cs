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
        [Required(ErrorMessage = "Mail is required.")]
        [EmailAddress(ErrorMessage = "Mail must be a valid email address.")]
        public string? Mail { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Host is required.")]
        public string? Host { get; set; }

        [Required(ErrorMessage = "Port is required.")]
        [Range(1, 65535, ErrorMessage = "Port must be a valid port number.")]
        public int Port { get; set; }

        public string? DisplayName { get; set; }
    }

    public class MailPaths
    {
        public Dictionary<string, string>? Paths { get; set; }
    }

    public class MailVerificationRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
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

    public class MailContractRequest
    {
        [Required]
        public string? Id { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        public string? ContractName { get; set; }

        [Required]
        public string? SigningDate { get; set; }

        [Required]
        public string? StartDate { get; set; }

        [Required]
        public string? TotalSalary { get; set; }
    }
}
