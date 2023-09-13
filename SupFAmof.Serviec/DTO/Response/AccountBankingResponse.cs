using System;
using System.Linq;
using System.Text;
using SupFAmof.Data.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SupFAmof.Service.DTO.Response
{
    public class AccountBankingResponse
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string Beneficiary { get; set; } = null!;
        public string AccountNumber { get; set; } = null!;
        public string BankName { get; set; } = null!;
        public string Branch { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
