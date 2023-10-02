
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Request.Account
{
    public class CreateAccountRequest
    {
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string? Name { get; set; }

        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string? Email { get; set; }

        [RegularExpression("^+?[0-9]+$", ErrorMessage = "Phone must contain only numeric characters.")]
        [MaxLength(15, ErrorMessage = "Phone cannot exceed 15 characters.")]
        public string? Phone { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = "DateOfBirth must be a valid DateTime.")]
        public DateTime? DateOfBirth { get; set; }

        public string? ImgUrl { get; set; }

    }

    public class CreateAccountReactivationRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "AccountId must be a positive integer.")]
        public int AccountId { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 50 characters.")]
        public string? Email { get; set; }
    }
}
