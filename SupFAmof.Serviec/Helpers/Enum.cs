using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SupFAmof.Service.Helpers
{
    public class Enum
    {
        public enum SystemAuthorize
        {
            [Display(Name = "Not Authorize")]
            NotAuthorize = -1,
        }

        public enum SystemRoleEnum
        {
            [Display(Name = "Cán bộ tuyển sinh")]
            AdmissionManager = 1,

            [Display(Name = "Sinh viên")]
            Student = 2,

            [Display(Name = "Những con báo kỳ 9")]
            SystemAdmin = 3,
        }

        public enum PostRegistrationStatusEnum
        {
            [Display(Name = "Pending")]
            Pending = 1,

            [Display(Name = "Confirm")]
            Confirm = 2,

            [Display(Name = "Reject")]
            Reject = 3,

            [Display(Name = "Cancel")]
            Cancel = 4,
        }
    }
}
