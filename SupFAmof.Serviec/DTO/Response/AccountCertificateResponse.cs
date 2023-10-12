using SupFAmof.Data.Entity;
using SupFAmof.Service.DTO.Response.Admission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Response
{
    public class AccountCertificateResponse
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual AccountResponse? CreatePerson { get; set; }
        public virtual AccountResponse? Account { get; set; }
        public virtual TrainingCertificateResponse? TraningCertificate { get; set; }

    }
}
