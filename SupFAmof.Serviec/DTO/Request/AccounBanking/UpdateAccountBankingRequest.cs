using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using static ServiceStack.LicenseUtils;
using System.ComponentModel.DataAnnotations;

namespace SupFAmof.Service.DTO.Request.AccounBanking
{
    public class UpdateAccountBankingRequest
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Beneficiary cannot exceed 50 characters.")]
        public string? Beneficiary { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Branch cannot exceed 50 characters.")]
        public string? Branch { get; set; }

        [Required]
        [RegularExpression("^[0-9]+$", ErrorMessage = "AccountNumber must contain only numeric characters.")]
        [MaxLength(50, ErrorMessage = "AccountNumber cannot exceed 50 characters.")]
        public string? AccountNumber { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "BankName cannot exceed 50 characters.")]
        public string? BankName { get; set; }

        [Required]
        [Range(typeof(bool), "true", "false", ErrorMessage = "IsActive must be either true or false.")]
        public bool IsActive { get; set; }
    }
}
