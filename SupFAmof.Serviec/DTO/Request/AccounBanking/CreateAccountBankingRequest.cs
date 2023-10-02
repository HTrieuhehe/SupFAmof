using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SupFAmof.Service.DTO.Request.AccounBanking
{
    public class CreateAccountBankingRequest
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Beneficiary cannot exceed 50 characters.")]
        public string? Beneficiary { get; set; }

        [Required]
        [RegularExpression("^[0-9]+$", ErrorMessage = "AccountNumber must contain only numeric characters.")]
        [MaxLength(50, ErrorMessage = "AccountNumber cannot exceed 50 characters.")]
        public string? AccountNumber { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "BankName cannot exceed 50 characters.")]
        public string? BankName { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Branch cannot exceed 50 characters.")]
        public string? Branch { get; set; }
    }
}
