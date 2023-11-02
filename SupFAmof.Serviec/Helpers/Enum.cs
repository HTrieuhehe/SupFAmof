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

            [Display(Name = "Cộng tác viên")]
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

            [Display(Name = "Check-in")]
            CheckIn = 7,

            [Display(Name = "Check-out")]
            CheckOut = 8
        }

        public enum PostStatusEnum
        {
            [Display(Name = "Opening")]
            Opening = 1,

            [Display(Name = "Close Post Registration of Post")]
            Avoid_Regist = 2,

            [Display(Name = "Ended")]
            Ended = 3,

            [Display(Name = "Cancel")]
            Cancel = 4,

            [Display(Name = "Deleted")]
            Delete = 5,

            [Display(Name = "Re open post registration of post")]
            Re_Open = 6
        }

        public enum PostPositionStatusEnum
        {
            [Display(Name = "Activated")]
            Active = 1,

            [Display(Name = "Deleted")]
            Delete = 2,
        }

        public enum EmailTypeEnum
        {
            [Display(Name = "VerificationMail")]
            VerificationMail = 1,

            [Display(Name = "BookingMail")]
            BookingMail = 2,

            [Display(Name = "ContractMail")]
            ContractMail = 2,
        }

        public enum AccountCertificateStatusEnum
        {
            [Display(Name = "Complete")]
            Complete = 1,

            [Display(Name = "Reject")]
            Reject = 2,
        }

        public enum AccountContractStatusEnum
        {
            [Display(Name = "Pending")]
            Pending = 1,

            [Display(Name = "Confirm")]
            Confirm = 2,

            [Display(Name = "Reject")]
            Reject = 3,

            [Display(Name = "Fail")]
            Fail = 4,
        }

        public enum ReportProblemStatusEnum
        {
            [Display(Name = "Pending")]
            Pending = 1,

            [Display(Name = "Approve")]
            Approve = 2,

            [Display(Name = "Reject")]
            Reject = 3,
        }

        public enum NotificationStatusEnum
        {
            [Display(Name = "Sent")]
            Sent = 1,

            [Display(Name = "Failed")]
            Failed = 2,
        }    
    }
}
