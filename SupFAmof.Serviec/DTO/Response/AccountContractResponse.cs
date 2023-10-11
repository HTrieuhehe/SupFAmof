using SupFAmof.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Response
{
    public class AccountContractResponse
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int AccountId { get; set; }
        public byte[]? SubmittedFile { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual AccountResponse? Account { get; set; }
        public virtual ContractResponse? Contract { get; set; }
    }
}
