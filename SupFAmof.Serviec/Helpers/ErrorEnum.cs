﻿using System;
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
            [Display(Name = "Invalid ACcount Personal ID")]
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

            //400
            [Display(Name = "Invalid API")]
            API_INVALID = 4006,

            //400
            [Display(Name = "Account cannot post")]
            POST_PERMIT_NOT_ALLOWED = 4007,

            //404
            [Display(Name = "Not Found Account")]
            ACCOUNT_NOT_FOUND = 404,

        }
        public enum AccountBankingErrorEnums
        {
            //400
            [Display(Name = "Invalid Account Banking")]
            ACCOUNT_INVALID = 4001,

            //400
            [Display(Name = "Invalid phone number")]
            ACCOUNT_PHONE_INVALID = 4002,

            //400
            [Display(Name = "Invalid Student Id")]
            ACCOUNT_STUDENTID_INVALID = 4003,

            //400
            [Display(Name = "Invalid Student Id")]
            ACCOUNT_BANKING_EXISTED = 4004,

            //404
            [Display(Name = "Not Found Account")]
            ACCOUNTBANKING_NOT_FOUND = 404,

        }

        public enum StaffErrorEnum
        {
            //404
            [Display(Name = "Staff ID not found!")]
            NOT_FOUND_ID = 4041,

            //400
            [Display(Name = "This staff already exsist!")]
            STAFF_EXSIST = 4001,

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

        }

        public enum PostTitleErrorEnum
        {
            //400
            [Display(Name = "Post Title existed!")]
            POST_TITLE_TYPE_EXISTED = 4001,

            //400
            [Display(Name = "Post Title Type invalid!")]
            INVALID_POST_TITLE_TYPE = 4001,

            //404
            [Display(Name = "Post Title not found!")]
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
            [Display(Name = "Time must be in from 3AM to 8PM or TimeTo must greater than TimeFrom")]
            INVALID_TIME_CREATE_POST = 4003,

            //404
            [Display(Name = "Post Not Found")]
            NOT_FOUND_ID = 4041,
        }
    }
}
