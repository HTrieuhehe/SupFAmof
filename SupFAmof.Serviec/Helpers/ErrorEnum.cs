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

        public enum FcmTokenErrorEnums
        {
            //404
            [Display(Name = "Invalid fcm token!")]
            INVALID_TOKEN = 4001
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
            ACCOUNT_INVALID = 400,

            //400
            [Display(Name = "Invalid ACcount Personal ID")]
            ACCOUNT_INVALID_PERSONAL_ID = 400,

            //400
            [Display(Name = "Account Information Existed!")]
            ACCOUNT_INFOMRATION_EXISTED = 400,

            //400
            [Display(Name = "Invalid phone number")]
            ACCOUNT_PHONE_INVALID = 400,

            //400
            [Display(Name = "Invalid Student Id")]
            ACCOUNT_STUDENTID_INVALID = 400,

            //404
            [Display(Name = "Not Found Account")]
            ACCOUNT_NOT_FOUND = 404,

        }
        public enum AccountBankingErrorEnums
        {
            //400
            [Display(Name = "Invalid Account")]
            ACCOUNT_INVALID = 400,

            //400
            [Display(Name = "Invalid phone number")]
            ACCOUNT_PHONE_INVALID = 400,

            //400
            [Display(Name = "Invalid Student Id")]
            ACCOUNT_STUDENTID_INVALID = 400,

            //404
            [Display(Name = "Not Found Account")]
            ACCOUNT_NOT_FOUND = 404,

        }

    }
}
