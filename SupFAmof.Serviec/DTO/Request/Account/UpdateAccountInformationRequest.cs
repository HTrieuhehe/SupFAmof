using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Request.Account
{
    public class UpdateAccountInformationRequest
    {
        public string? IdentityNumber { get; set; }
        public string? IdStudent { get; set; }
        public string? FbUrl { get; set; }
        public string? Address { get; set; }
        public DateTime? IdentityIssueDate { get; set; }
        public string? PlaceOfIssue { get; set; }
        public string? IdentityFrontImg { get; set; }
        public string? IdentityBackImg { get; set; }
        public string? TaxNumber { get; set; }
    }
}
