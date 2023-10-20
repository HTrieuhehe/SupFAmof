using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SupFAmof.Service.DTO.Response.Admission
{
    public class AdmissionDocumentResponse
    {
        public int? Id { get; set; }
        public string? DocName { get; set; }
        public string? DocUrl { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreateAt { get; set; }
    }
}
