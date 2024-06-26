﻿using SupFAmof.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Response.Admission
{
    public class AdmissionPostResponse
    {
        public AdmissionPostResponse()
        {
            RegisterAmount = 0;
            TotalAmountPosition = 0;
            TotalUpdateRegisterAmount = 0;
            TotalRegisterAmount = 0;
            AnyRegister = false;
        }

        public int? Id { get; set; }
        public int? AccountId { get; set; }
        public int? PostCategoryId { get; set; }
        public string? PostCode { get; set; }
        public string? PostImg { get; set; }
        public string? PostDescription { get; set; }
        public int? Priority { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool? IsPremium { get; set; }
        public int? Status { get; set; }
        public bool? AttendanceComplete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public int? RegisterAmount;
        public int? TotalRegisterAmount;
        //public  int? RegisterAmount { get => registerAmount ?? 0; set => registerAmount = value; }

        public int? TotalAmountPosition;
        //public  int? TotalAmountPosition { get => totalAmountPosition ?? 0; set => totalAmountPosition = value; }

        public int? TotalUpdateRegisterAmount;
        public bool? AnyRegister;

        public AccountResponse? Account { get; set; }
        public PostCategoryResponse? PostCategory { get; set; }
        public ICollection<PostPositionResponse>? PostPositions { get; set; }
    }
}
