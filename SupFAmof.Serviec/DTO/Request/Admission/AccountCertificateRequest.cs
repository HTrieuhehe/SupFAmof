using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Request.Admission
{
    public class CreateAccountCertificateRequest
    {
        //public int Id { get; set; }
        public int AccountId { get; set; }
        //public int? CreatePersonId { get; set; }
        public int TraningCertificateId { get; set; }
        //public bool Status { get; set; }
        //public DateTime CreateAt { get; set; }
        //public DateTime? UpdateAt { get; set; }
    }
}
