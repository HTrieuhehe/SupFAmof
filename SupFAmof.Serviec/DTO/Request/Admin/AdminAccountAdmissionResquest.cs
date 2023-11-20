using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ServiceStack.LicenseUtils;

namespace SupFAmof.Service.DTO.Request.Admin
{
    public class UpdateAdminAccountAdmissionRequest
    {
        [Required(ErrorMessage = "Admission account Id is required")]
        public int? Id { get; set; }

        [Required(ErrorMessage = "Post Permission status (True or False) is required")]
        public bool? PostPermission { get; set; }
    }
}
