using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SupFAmof.Service.DTO.Request.AccounBanking
{
    public class CreateAccountBankingRequest
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; } = null!;
        public string BankName { get; set; } = null!;

    }
}
