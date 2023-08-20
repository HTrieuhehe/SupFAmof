using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Request.Admission
{
    public class CreateTrainingCertificateRequest
    {
        //public int Id { get; set; }
        public string? TrainingTypeId { get; set; }
        public string? CertificateName { get; set; }
        //public DateTime CreateAt { get; set; }
        //public DateTime? UpdateAt { get; set; }
    }

    public class UpdateTrainingCertificateRequest
    {
        //public int Id { get; set; }
        public string? TrainingTypeId { get; set; }
        public string? CertificateName { get; set; }
        //public DateTime CreateAt { get; set; }
        //public DateTime? UpdateAt { get; set; }
    }
}
