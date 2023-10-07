using SupFAmof.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Response.Admission
{
    public class AdmissionContractResponse
    {
        public int Id { get; set; }
        public string ContractName { get; set; }
        public string ContractDescription { get; set; }
        public byte[] SampleFile { get; set; }
        public DateTime SigningDate { get; set; }
        public DateTime StartDate { get; set; }
        public double TotalSalary { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual ICollection<AccountContract> AccountContracts { get; set; }
    }
}
