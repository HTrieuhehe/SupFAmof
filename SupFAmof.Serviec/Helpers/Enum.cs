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
            [Display(Name = "Check-in")]
            CheckIn = 5,
            [Display(Name = "Check-out")]
            CheckOut = 6,
            [Display(Name = "Quit")]
            Quit = 7
        }
        public enum PostRGUpdateHistoryEnum
        {
            [Display(Name = "Pending")]
            Pending = 1,
            [Display(Name = "Reject")]
            Rejected = 3,
            [Display(Name = "Approve")]
            Approved = 2
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
            ContractMail = 3,
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

        public enum NotificationTypeEnum
        {
            [Display(Name = "New Upcoming Event")]
            Post_Created = 1,

            [Display(Name = "Post Registration Confirm")]
            PostRegistration_Confirm = 2,

            [Display(Name = "Bing Bong! New contract to you")]
            Contract_Request = 3,

            [Display(Name = "Bing Bong! Check out complete")]
            Check_out_complete = 4,

            [Display(Name = "Bing Bong! Post is re-opened")]
            Post_Re_Opened = 5,

            [Display(Name = "Application Request")]
            Application = 6,
        }
    }
}
