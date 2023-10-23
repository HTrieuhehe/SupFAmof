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

        public enum ExpoPushTokenErrorEnum
        {
            //404
            [Display(Name = "Invalid Expo token!")]
            INVALID_TOKEN = 400
        }

        public enum RoleErrorEnums
        {
            //400
            [Display(Name = "Invalid Role")]
            ROLE_INVALID = 400,

            //404
            [Display(Name = "Not Found Role")]
            ROLE_NOTE_FOUND = 404,
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
            [Display(Name = "You did not have permission to use this function")]
            PERMISSION_NOT_ALLOW = 4013,

            //404
            [Display(Name = "Not Found Account")]
            ACCOUNT_NOT_FOUND = 404,

            //500
            [Display(Name = "Server are busy")]
            SERVER_BUSY = 500,

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
            [Display(Name = "Account Number cannot null")]
            ACCOUNT_BAKING_NUMBER_NOT_NULL = 4004,

            //404
            [Display(Name = "Not Found Account")]
            ACCOUNTBANKING_NOT_FOUND = 404,

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
        }

        public enum PostRegistrationErrorEnum
        {
            //404
            [Display(Name = "Post Registration not found!")]
            NOT_FOUND_POST = 4041,

            //400
            [Display(Name = "UPDATE POST REGISTRATION FAILED")]
            UPDATE_FAILED_POST = 4000,

            //400
            [Display(Name = "EXCEEDING TIME LIMIT ")]
            EXCEEDING_TIME_LIMIT = 4001,

            //400
            [Display(Name = "APPROVE OR DISAPPROVE MUST BE PROVIDED")]
            APPROVE_OR_DISAPPROVE = 4002,
            //400
            [Display(Name = "FULL SLOT")]
            FULL_SLOT = 4003,
            //400
            [Display(Name = "CANT REGISTER THE SAME POST OR POST THAT YOU ARE REGISTER HAS THE SAME DAY AS ANOTHER EVENT YOU REGISTER")]
            ALREADY_REGISTERED = 4004,

            //400
            [Display(Name = "UPDATE REQUEST HAS ALREADY BEEN APPROVED")]
            ALREADY_APPROVE = 4005,

            //400
            [Display(Name = "UPDATE REQUEST HAS ALREADY BEEN REJECTED")]
            ALREADY_REJECT = 4006,
            //400
            [Display(Name = "MUST SENT REGISTER 1 DAY BEFORE THE EVENT")]
            OUTDATED_REGISTER = 4007,

            //400
            [Display(Name = "School Bus option is not qualified ")]
            NOT_QUALIFIED_SCHOOLBUS = 4008,
            //400
            [Display(Name = "Can not registrate cause you created this post")]
            POST_CREATOR = 4009,

            //400
            [Display(Name = "Cant not approve the same id twice")]
            DUPLICATE_IDS = 4010,

            //404
            [Display(Name = "No registration need to be update")]
            NOT_FOUND_UPDATE_REGISTRATION_REQUEST = 4011,

            //404
            [Display(Name = "Need certificate to register this position")]
            NOT_FOUND_CERTIFICATE = 4012,

            //400
            [Display(Name = "This post is done")]
            POST_OUTDATED = 4013,
            //400
            [Display(Name = "Postion work time is duplicated to one that you are attending")]
            DUPLICATE_TIME_POSTION = 4014,


        }

        public enum PostCategoryErrorEnum
        {
            //400
            [Display(Name = "Post Category existed!")]
            POST_TITLE_TYPE_EXISTED = 4001,

            //400
            [Display(Name = "Post Category Type is duplication!")]
            POST_TITLE_TYPE_DUPLICATE = 4002,

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
            [Display(Name = "Account Certificate not found!")]
            NOT_FOUND_ID = 4041,
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
            [Display(Name = "Cannot delete because there are one or more people applied to this post/position")]
            UPDATE_FAIl = 4006,

            //404
            [Display(Name = "Post Not Found")]
            NOT_FOUND_ID = 4041,

            //404
            [Display(Name = "Post Position Not Found")]
            POSITION_NOT_FOUND = 4042,

            //404
            [Display(Name = "Post Position Not Found")]
            INVALID_END_POST = 4043,
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
            [Display(Name = "Distance too far")]
            DISTANCE_TOO_FAR = 4006,
            //400
            [Display(Name = "Wrong information (post or position not exist)")]
            WRONG_INFORMATION = 4007,
            //500
            [Display(Name = "Position longtitude and latitude is not specified to perform checkin")]
            MISSING_INFORMATION_POSITION = 4008,
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
            [Display(Name = "Account has been ban until")]
            CREATE_BANNED_INVALID = 4001,


            //403
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
            //400
            [Display(Name = "You are not verified by Admin to allow to create contract")]
            ACCOUNT_CREATE_CONTRACT_INVALID = 4001,

            //400
            [Display(Name = "Siging date cannot be greater than Start Date")]
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
            [Display(Name = "Not found thid Account Report")]
            NOT_FOUND_REPORT = 4041,
        }
    }
}
