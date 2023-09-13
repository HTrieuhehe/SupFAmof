using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;

namespace SupFAmof.Service.DTO.Request.AccounBanking
{
    public class CreateAccountBankingRequest
    {
        public string? Beneficiary { get; set; }

        [Required]
        public string? AccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? Branch { get; set; }
    }
}
