using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SupFAmof.Service.DTO.Request.AccounBanking
{
    public class CreateAccountBankingRequest
    {
        public string? AccountNumber { get; set; } 
        public string? BankName { get; set; }
    }
}
