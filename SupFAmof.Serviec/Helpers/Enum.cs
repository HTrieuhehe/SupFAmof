using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
            Collaborator = 2,

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

            [Display(Name = "Update Request")]
            Update_Request = 5,

            [Display(Name = "Approved Request")]
            Approved_Request = 6,
        }

        public enum PostStatusEnum
        {
            [Display(Name = "Opening")]
            Opening = 1,

            [Display(Name = "Closed")]
            Closed = 2,

            [Display(Name = "Ended")]
            Ended = 3,

            [Display(Name = "Cancel")]
            Cancel = 4,
        }
    }
}
