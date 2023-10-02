using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ServiceStack.LicenseUtils;

namespace SupFAmof.Service.DTO.Request.Account
{
    public class CreateAccountInformationRequest
    {
        [RegularExpression("^[0-9]+$", ErrorMessage = "IdentityNumber must contain only numeric characters.")]
        [MaxLength(20, ErrorMessage = "IdentityNumber cannot exceed 20 characters.")]
        public string? IdentityNumber { get; set; }

        [MaxLength(10, ErrorMessage = "IdStudent cannot exceed 10 characters.")]
        public string? IdStudent { get; set; }

        [MaxLength (225, ErrorMessage = "FbUrl cannot exceed 225 characters.")]
        public string? FbUrl { get; set; }

        [MaxLength(225, ErrorMessage = "Address cannot exceed 225 characters.")]
        public string? Address { get; set; }

        [DataType(DataType.DateTime, ErrorMessage = "IdentityIssueDate must be a valid DateTime.")]
        public DateTime? IdentityIssueDate { get; set; }

        [MaxLength(100, ErrorMessage = "PlaceOfIssue cannot exceed 100 characters.")]
        public string? PlaceOfIssue { get; set; }

        public string? IdentityFrontImg { get; set; }
        public string? IdentityBackImg { get; set; }

        [RegularExpression("^[0-9]+$", ErrorMessage = "TaxNumber must contain only numeric characters.")]
        [MaxLength(50, ErrorMessage = "TaxNumber cannot exceed 50 characters.")]
        public string? TaxNumber { get; set; }    
    }
}
