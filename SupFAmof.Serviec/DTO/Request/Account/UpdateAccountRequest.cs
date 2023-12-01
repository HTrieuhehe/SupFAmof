using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ServiceStack.LicenseUtils;

namespace SupFAmof.Service.DTO.Request.Account
{
    public class UpdateAccountRequest
    {
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string? Name { get; set; }

        [RegularExpression("^+?[0-9]+$", ErrorMessage = "Phone must contain only numeric characters.")]
        [MaxLength(15, ErrorMessage = "Phone cannot exceed 15 characters.")]
        public string? Phone { get; set; }

        //[DataType(DataType.DateTime, ErrorMessage = "DateOfBirth must be a valid DateTime.")]
        public DateTime? DateOfBirth { get; set; }

        public string? ImgUrl { get; set; }

        public UpdateAccountInformationRequest? AccountInformation { get; set; }
    }

    public class UpdateAccountAvatar
    {
        [Required]
        public string? ImgUrl { get; set; }
    }

    public class UpdateCitizenIdentificationFrontImg
    {
        public string? IdentityFrontImg { get; set; }
    }

    public class UpdateCitizenIdentificationBackImg
    {
        public string? IdentityBackImg { get; set; }
    }

    public class UpdateCitizenIdentification
    {
        public string? IdentityNumber { get; set; }
        public string? Address { get; set; }
        public DateTime? IdentityIssueDate { get; set; }
        public string? PlaceOfIssue { get; set; }
    }
}
