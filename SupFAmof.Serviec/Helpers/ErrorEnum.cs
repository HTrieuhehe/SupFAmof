using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SupFAmof.Service.Helpers
{
    public class ErrorEnum
    {
        //400 Bad Request
        //404 Not Found
        //403 Forbidden

        public enum ExpoPushTokenErrorEnum
        {
            //404
            [Display(Name = "Invalid Expo token!")]
            INVALID_TOKEN = 4001
        }

        public enum RoleErrorEnums
        {
            //400
            [Display(Name = "Invalid Role")]
            ROLE_INVALID = 4001,

            //404
            [Display(Name = "Not Found Role")]
            ROLE_NOTE_FOUND = 4041,
        }

        public enum AdminErrorEnums
        {
            //404
            [Display(Name = "Administrator not found")]
            ADMIN_NOT_FOUND = 4041,
        }

        public enum AdmissionCredentialErrorEnums
        {
            //400
            [Display(Name = "Admission credential already upgrade")]
            ALREADY_UPGRADE = 4001,

            //400
            [Display(Name = "Admission credential already disable")]
            ALREADY_DISABLE = 4002,
        }

        public enum AccountErrorEnums
        {
            //400
            [Display(Name = "Invalid Account")]
            ACCOUNT_INVALID = 4001,

            //400
            [Display(Name = "Invalid Account Personal ID")]
            ACCOUNT_INVALID_PERSONAL_ID = 4002,

            //400
            [Display(Name = "Account Information Existed!")]
            ACCOUNT_INFOMRATION_EXISTED = 4003,

            //400
            [Display(Name = "Invalid phone number")]
            ACCOUNT_PHONE_INVALID = 4004,

            //400
            [Display(Name = "Invalid Student Id")]
            ACCOUNT_STUDENTID_INVALID = 4005,

            //403
            [Display(Name = "You are not allow to use this API")]
            API_INVALID = 4031,

            //400
            [Display(Name = "Account cannot post")]
            POST_PERMIT_NOT_ALLOWED = 4007,

            //400
            [Display(Name = "Account Avatar URL cannot empty or null")]
            ACCOUNT_AVATAR_URL_INVALID = 4008,

            //400
            [Display(Name = "Account has already disable")]
            ACCOUNT_DISABLE = 4009,

            //400
            [Display(Name = "Account has already enable")]
            ACCOUNT_DOES_NOT_DISABLE = 4010,

            //400
            [Display(Name = "Verify code invalid")]
            VERIFY_CODE_INVALID = 4011,

            //400
            [Display(Name = "Cannot update multiple time! Try after 5 minutes")]
            UPDATE_INVALUD = 4012,

            //400
            [Display(Name = "AccountId cannot null or empty")]
            ACCOUNT_ID_NOT_NULL = 4013,

            //400
            [Display(Name = "CCCD is duplicated")]
            IDENTIFICATION_DUPLICATE = 4014,

            //400
            [Display(Name = "Student is duplicated")]
            STUDENT_ID_DUPLICATE = 4015,

            //400
            [Display(Name = "Phone number is duplicated")]
            PHONE_NUM_DUPLICATE = 4016,

            //400
            [Display(Name = "Tax number is duplicated")]
            TAX_NUM_DUPLICATE = 4017,

            //400
            [Display(Name = "Back side of of your card is not belong to the front side")]
            WRONG_BACK_CARD = 4018,

            //400
            [Display(Name = "Collaborator is not currently premium")]
            INVALID_CREDENTIAL = 4019,

            //400
            [Display(Name = "Identification card number allow only number")]
            CARD_CONTAINT_ONLY_NUMBER = 4020,

            //400
            [Display(Name = "You are using CMND with 9 digit number! Upgrade to CCCD to scan")]
            CCCD_INVALID_VERSION = 4021,

            //403
            [Display(Name = "You did not have permission to use this function")]
            PERMISSION_NOT_ALLOW = 4032,

            //403
            [Display(Name = "You are banned! Now you can not do anything unitl banned is over!")]
            BANNED_IN_PROCESS = 4033,

            //404
            [Display(Name = "Not Found Account")]
            ACCOUNT_NOT_FOUND = 4041,

            //404
            [Display(Name = "Not Found Account")]
            COLLABORATOR_NOT_FOUND = 4042,

            //500
            [Display(Name = "Server are busy")]
            SERVER_BUSY = 5001,
        }

        public enum AccountBankingErrorEnums
        {
            //400
            [Display(Name = "Invalid Account Banking")]
            ACCOUNT_INVALID = 4001,

            //400
            [Display(Name = "Account Banking Information Existed")]
            ACCOUNT_BANKING_EXISTED = 4002,

            //400
            [Display(Name = "Account Number must contain only number or cannot null")]
            ACCOUNT_BAKING_NUMBER_INVALID = 4003,

            //400
            [Display(Name = " is required!")]
            FIELD_REQUIRED = 4004,

            //404
            [Display(Name = "Not Found Account")]
            ACCOUNTBANKING_NOT_FOUND = 4041,

        }

        public enum AdminAccountErrorEnum
        {
            //404
            [Display(Name = "Staff ID not found!")]
            NOT_FOUND_ID = 4041,

            //400
            [Display(Name = "This staff already exsist!")]
            ADMIN_EXSIST = 4001,

            //400
            [Display(Name = "Username or password is not correct")]
            LOGIN_FAIL = 4002,

            //403
            [Display(Name = "Admin is unable to use this function")]
            ADMIN_FORBIDDEN = 4031,
        }

        public enum PostRegistrationErrorEnum
        {
            //404
            [Display(Name = "Post Registration not found!")]
            NOT_FOUND_POST = 4041,

            //400
            [Display(Name = "Post Registration update fail")]
            UPDATE_FAILED_POST = 4000,

            //400
            [Display(Name = "Exceeding time limit ")]
            EXCEEDING_TIME_LIMIT = 4001,

            //400
            [Display(Name = "Approve or disapprove must be provided")]
            APPROVE_OR_DISAPPROVE = 4002,

            //400
            [Display(Name = "Slot already full!")]
            FULL_SLOT = 4003,

            //400
            [Display(Name = "Your registration overlaps with another registration time")]
            ALREADY_REGISTERED = 4004,

            //400
            [Display(Name = "Update request has already been approved")]
            ALREADY_APPROVE = 4005,

            //400
            [Display(Name = "Update request has already been rejected")]
            ALREADY_REJECT = 4006,

            //400
            [Display(Name = "School Bus option is not qualified ")]
            NOT_QUALIFIED_SCHOOLBUS = 4008,

            //400
            [Display(Name = "Can not registrate cause you created this post")]
            POST_CREATOR = 4009,

            //400
            [Display(Name = "Cant not approve the same id twice")]
            DUPLICATE_IDS = 4010,

            //400
            [Display(Name = "No registration need to be update")]
            NOT_FOUND_UPDATE_REGISTRATION_REQUEST = 4011,

            //400
            [Display(Name = "Need certificate to register this position")]
            NOT_FOUND_CERTIFICATE = 4012,

            //400
            [Display(Name = "This post is done")]
            POST_OUTDATED = 4013,

            //400
            [Display(Name = "Postion work time is duplicated to one that you are attending")]
            DUPLICATE_TIME_POSTION = 4014,

            //404
            [Display(Name = "Postion not found")]
            POSITION_NOTFOUND = 4015,

            //400
            [Display(Name = "This position is done")]
            POSITION_OUTDATED = 4016,

            //400
            [Display(Name = "Post Registration Id cannot null or empty")]
            POST_REGISTRATION_CANNOT_NULL_OR_EMPTY = 4017,
            //400
            [Display(Name = "Post is either closed or deleted")]
            POST_NOT_AVAILABLE = 4018,
            //400
            [Display(Name = "Cant be updated ")]
            CANT_BE_UPDATED = 4019,
            //400
            [Display(Name = "Already sent update request.Only request one time ")]
            DUPLICATED_REQUEST_UPDATE = 4020,
            //400
            [Display(Name = "Can only update position in that post")]
            WRONG_POSITION = 4021,
            //400
            [Display(Name = "Already cancelled")]
            ALREADY_CANCELLED = 4022,
            //400
            [Display(Name = "Already register this position.Cannot update to this position")]
            UPDATE_FAILED = 4023,
            //400
            [Display(Name = "Sorry.To prevent spamming we are limiting to one request for the same work day and time ")]
            REQUEST_FAILED = 4024,
            //400
            [Display(Name = "Must wait for the update request be reiviewed before send a new one ")]
            DUPLICATE_PENDING = 4025,

            //403
            [Display(Name = "Account is banned")]
            ACCOUNT_BANNED = 4026,
            //400
            [Display(Name = "Cancel Failed")]
            CANCEL_FAILED = 4027,
            //400
            [Display(Name = "MUST WAIT FOR THE PREVIOUS BE REVIEWED ")]
            OVERLAP_APPLICATION_SEND = 4028,
            //400
            [Display(Name = "Same time with the interview day")]
            OVERLAP_TRAINING_EVENT_DAY = 4029,
        }

        public enum PostCategoryErrorEnum
        {
            //400
            [Display(Name = "Post Category existed!")]
            POST_CATEGORY_TYPE_EXISTED = 4001,

            //400
            [Display(Name = "Post Category Type is duplicate!")]
            POST_CATEGORY_TYPE_DUPLICATE = 4002,

            //404
            [Display(Name = "Post Category not found!")]
            NOT_FOUND_ID = 4041,
        }

        public enum TrainingCertificateErrorEnum
        {
            //400
            [Display(Name = "Training Certificate existed!")]
            TRAINING_CERTIFICATE_EXISTED = 4001,

            //400
            [Display(Name = "Training Certificate Type invalid!")]
            INVALID_TRAINING_CERTIFICATE_TYPE = 4002,

            //404
            [Display(Name = "Training Certificate not found!")]
            NOT_FOUND_ID = 4041,
            //400
            [Display(Name = "Cant create interview from the past")]
            PAST_CLASS = 4003,
            //400
            [Display(Name = "Cant register")]
            REGISTER_FAILED = 4004,
            //400
            [Display(Name = "Duplicate registration")]
            DUPLICATE_REGISTRATION = 4006,
            //400
            [Display(Name = "Same time with different training  day")]
            OVERLAP_INTERVIEW = 4007,
            //400
            [Display(Name = "Interview does not exist")]
            TRAINING_DAY_DOES_NOT_EXIST = 4008,
            //400
            [Display(Name = "Only check attedance on the day of the interview and on time")]
            CANT_CHECK_ATTEDANCE = 4009,
            //400
            [Display(Name = "Certificate registration not found")]
            CERTIFICATE_REGISTRATION_NOT_FOUND = 4010,
            //400
            [Display(Name = "Already have this certificate")]
            ALREADY_HAVE_CERTIFICATE = 4011,
            //400
            [Display(Name = "Cant update due to it having already assigned to collaborator")]
            CANT_UPDATE = 4012,
            //400
            [Display(Name = "Cant assign past interview date to collab")]
            PAST_INTERVIEW = 4013,
            //400
            [Display(Name = "Interview day matched with collab work time")]
            MATCHED_WORK_TIME = 4014,
            //400
            [Display(Name = "Can't assign same interview ")]
            SAME_INTERVIEW = 4015,
            //400
            [Display(Name = "Interview time must be more than 1 hour")]
            INTERVIEW_TIME_1_HOUR = 4016,
            //400
            [Display(Name = "Either the time is in the pass or you can't assign due to it being only one hour to assign")]
            FAILED_TO_ASSIGN = 4017,
            [Display(Name = "Either the time is in the pass or you must create 3 hour more than you current time")]
            FAILED_TO_CREATE = 4018,
            [Display(Name = "Can't assign")]
            CANT_ASIGN = 4019,
            [Display(Name = "Interview Day is in the past or if you assign now collab wont have enough time to prepare")]
            INTERVIEW_PAST  = 4020,
        }

        public enum AccountCertificateErrorEnum
        {
            //400
            [Display(Name = "Account Certificate existed!")]
            ACCOUNT_CERTIFICATE_EXISTED = 4001,

            //400
            [Display(Name = "Create Person invalid!")]
            INVALID_CREATE_PERSON = 4002,

            //400
            [Display(Name = "Collaborator Certificate has not changing")]
            STATUS_ALREADY_SAME = 4003,

            //400
            [Display(Name = "List of Certificates has no data")]
            CERTIFICATE_LIST_EMPTY = 4004,

            //400
            [Display(Name = "You are not the issuer! ")]
            WRONG_ISSUER_PERSON = 4005,

            //404
            [Display(Name = "Account Certificate not found!")]
            NOT_FOUND_ID = 4041,

            //404
            [Display(Name = "Account is not collaborator")]
            ACCOUNT_COLLABORATOR_INVALID = 4042,
        }

        public enum PostErrorEnum
        {
            //400
            [Display(Name = "Post Invalid")]
            INVALID_POST = 4001,

            //400
            [Display(Name = "Create Post Date Invalid")]
            INVALID_DATE_CREATE_POST = 4002,

            //400
            [Display(Name = "Date To must equal or greater than Date From")]
            INVALID_DATETIME_CREATE_POST = 4003,

            //400
            [Display(Name = "Time must be in from 3AM to 8PM or TimeTo must greater than TimeFrom")]
            INVALID_TIME_CREATE_POST = 4004,

            //400
            [Display(Name = "Post are missing collaborator! Cannot Ended!")]
            INVALID_RUN_POST = 4005,

            //400
            [Display(Name = "Cannot delete because there are one or more people applied to this post")]
            DELETE_POST_FAIL = 4006,

            //400
            [Display(Name = "Recruitment post is not allowed to re open")]
            INVALID_RE_OPEN_POST = 4007,

            //400
            [Display(Name = "Your event is running or reach limit time of 30 minutes before running")]
            RE_OPEN_POST_FAIL = 4008,

            //400
            [Display(Name = "Date in position must in range of Date From and Date To in post")]
            INVALID_POSITION_DATE = 4009,

            //400
            [Display(Name = "New position cannot be null")]
            INVALID_NEW_POSITION = 40010,

            //400
            [Display(Name = "Missing position valid with post end time")]
            VALID_POSITION_MISSING = 40011,

            //400
            [Display(Name = "Cannot delete because there are one or more people applied to this position")]
            DELETE_POSITION_FAIL = 40012,

            //400
            [Display(Name = "Recruitment post is not allowed to re open")]
            INVALID_RE_OPEN = 40013,

            //400
            [Display(Name = "has been deleted so it can not be edited")]
            POSITION_EDITED_FORBIDDEN = 40014,

            //400
            [Display(Name = "Position date must in range of post")]
            POSITION_DATE_UPDATE_INALID = 40015,

            //400
            [Display(Name = "Position amount must higher than: ")]
            AMOUNT_INVALID = 40016,

            //403
            [Display(Name = "You are not premium to see this post!")]
            PREMIUM_REQUIRED = 4031,

            //404
            [Display(Name = "Post Not Found")]
            NOT_FOUND_ID = 4041,

            //404
            [Display(Name = "Post Position Not Found")]
            POSITION_NOT_FOUND = 4042,

            //404
            [Display(Name = "Cannot end post if there is any position is running")]
            INVALID_END_POST = 4043,

            //408
            [Display(Name = "Request Time Out!")]
            REQUEST_TIME_OUT = 4081,
        }

        public enum DocumentErrorEnum
        {
            //400
            [Display(Name = "Invalid Document")]
            INVALID_DOCUMENT = 4001,

            //404
            [Display(Name = "Document not found")]
            NOT_FOUND_DOCUMENT = 4002,

            //400
            [Display(Name = "Already disabled")]
            ALREADY_DISABLED = 4003,
        }

        public enum AttendanceErrorEnum
        {
            //400
            [Display(Name = "Check-in fail!")]
            CHECK_IN_FAIL = 4001,

            //400
            [Display(Name = "Account did not check in!")]
            CHECK_OUT_FAIL = 4002,

            //400
            [Display(Name = "Check out time invalid!")]
            CHECK_OUT_TIME_INVALID = 4003,

            //400
            [Display(Name = "Cannot check out")]
            CAN_NOT_CHECK_OUT = 4004,

            //400
            [Display(Name = "Already Check in")]
            ALREADY_CHECK_IN = 4005,

            //400
            [Display(Name = "You are far from check in location: ")]
            DISTANCE_TOO_FAR = 4006,

            //400
            [Display(Name = "Wrong information (post or position not exist)")]
            WRONG_INFORMATION = 4007,

            //400
            [Display(Name = "You are not check in yet")]
            NOT_CHECK_IN_YET = 4008,

            //401
            [Display(Name = "Position longtitude and latitude is not specified to perform checkin")]
            MISSING_INFORMATION_POSITION = 4011,
        }

        public enum AccountBannedErrorEnum
        {
            //404
            [Display(Name = "Banned Information Not Found!")]
            NOT_FOUND_BANNED_ACCOUNT = 4041,

            //403
            [Display(Name = "Only Admission Officer have permission to post can ban account")]
            ADMISSION_FORBIDDEN = 4031,

            //403
            [Display(Name = "Collaborator is not banned by you")]
            ADMISSION_INVALID = 4032,

            //400
            [Display(Name = "Account has been ban until")]
            CREATE_BANNED_INVALID = 4001,


            //400
            [Display(Name = "Ending Day must equal or higher than")]
            DAY_END_INVALID = 4002,
        }

        public enum MailErrorEnum
        {
            //400
            [Display(Name = "Failed to send email")]
            SEND_MAIL_FAIL = 4001,
        }

        public enum ContractErrorEnum
        {
            //403
            [Display(Name = "You are not verified by Admin to allow to create contract")]
            ACCOUNT_CREATE_CONTRACT_INVALID = 4031,

            //403
            [Display(Name = "You are not the admission officer who create this contract")]
            CONTRACT_CREATE_PERSON_NOT_ALLOW = 4032,

            //400
            [Display(Name = "Signing date cannot be greater than Start Date")]
            SIGNING_DATE_INVALID_WITH_START_DATE = 4002,

            //400
            [Display(Name = "Signing date must at least 2 days greater than current date")]
            SIGNING_DATE_INVALID_WITH_CURRENT_DATE = 4003,

            //400
            [Display(Name = "Start date cannot be less than Signing Date")]
            START_DATE_INVALID_WITH_SIGNING_DATE = 4004,

            //400
            [Display(Name = "Cannot update contract because there are one or more accounts confirm your contract")]
            UPDATE_CONTRACT_INVALID = 4005,

            //400
            [Display(Name = "Cannot disable contract because there are one or more accounts confirm your contract")]
            DISABLE_CONTRACT_INVALID = 4006,

            //400
            [Display(Name = "End Date cannot be less than Start Date")]
            END_DATE_INVALID_WITH_START_DATE = 4007,

            //400
            [Display(Name = "End Date must be within 30 days: ")]
            END_DATE_INVALID = 4008,

            //404
            [Display(Name = "Contract not found")]
            NOT_FOUND_CONTRACT = 4041,
        }

        public enum SendEmailContractErrorEnum
        {

        }

        public enum ComplaintErrorEnum
        {
            //404
            [Display(Name = "Your Report Application Not Found")]
            NOT_FOUND_REPORT = 4041,
            //400
            [Display(Name = "Report application is already approved")]
            ALREADY_APPROVE = 4042,
            //404
            [Display(Name = "Report application is already rejected")]
            ALREADY_REJECT = 4043,
        }

        public enum AccountReportErrorEnum
        {
            //404
            [Display(Name = "Not found this Account Report")]
            NOT_FOUND_REPORT = 4041,

            //500
            [Display(Name = "Does not have enough information to generate report")]
            MISSING_INFORMATION = 4042,

            //400
            [Display(Name = "Must have post permission to view ")]
            UNAUTHORIZED = 4001,
        }

        public enum AccountContractErrorEnum
        {
            //400
            [Display(Name = "Collaborator has confirm or reject contract before")]
            CONTRACT_ACCOUNT_ALREADY_UPDATE = 4001,

            //400
            [Display(Name = "Right now you already have a contract! Cannot confirm more!")]
            CONTRACT_ALREADY_CONFIRM = 4002,

            //400
            [Display(Name = "No Collaborator meet the need to send request")]
            OVER_COLLABORATOR = 4003,

            //400
            [Display(Name = "Collaborator error handling")]
            COLLABORATOR_ERROR_HANDLING = 4004,

            //400
            [Display(Name = "Collaborator mising account banking information")]
            MISSING_BANKING_INFORMATION = 4005,

            //400
            [Display(Name = "Account Contract is not available to complete")]
            COMPLETE_INVALID = 4006,

            //400
            [Display(Name = "Cannot confirm Contract after 5pm of signing date")]
            CONFIRM_INVALID = 4007,

            //400
            [Display(Name = "Upload file failed")]
            UPLOADED_FAILED = 4008,

            //400
            [Display(Name = "Cannot complete the contract until")]
            CANNOT_COMPLETE_CONTRACT = 4009,

            //404
            [Display(Name = "Error occur when collaborator not confirm or reject")]
            CONTRACT_REMOVED_ADMISSION = 4041,

            //404
            [Display(Name = "Account Contract Not Found")]
            NOT_FOUND_ACCOUNT_CONTRACT = 4042,
        }

        public enum CheckAttendanceErrorEnum
        {
            //404
            [Display(Name = "Attendance Not Found")]
            ATTENDANCE_NOT_FOUND = 4041,
            //400
            [Display(Name = "Confirm time expired")]
            CONFIRM_TIME_EXPIRED = 4001,
            //400
            [Display(Name = "You cant update status because wrong position ")]
            CANT_UPDATE_WRONG_POSITION = 4002,
            //400
            [Display(Name = "Confirm time not begin")]
            CONFIRM_TIME_NOT_BEGIN = 4003,
        }

        public enum AdminSystemManagementErrorEnum
        {
            //400
            [Display(Name = "Role email is not necessary to have @ character")]
            CHARACTER_INVALID = 4001,

            //400
            [Display(Name = "Role email is not necessary to have @ character")]
            SYSTEM_ROLE_NOT_FOUND = 4041,
        }

        public enum ApplicationErrorEnum
        {
            //400
            [Display(Name = "Problem Note is required.")]
            PROBLEM_NOTE_EMPTY = 4001,

            //400
            [Display(Name = "Problem Note exeed 500 characters")]
            PROBLEM_NOTE_EXEED = 4002,
        }
    }
}
