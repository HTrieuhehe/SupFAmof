﻿using System;
using System.Linq;
using System.Text;
using SupFAmof.Data.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SupFAmof.Service.DTO.Response
{
    public class PostRegistrationResponse
    {
        public int Id { get; set; }
        public string RegistrationCode { get; set; } = null!;
        public int Status { get; set; }
        public bool? SchoolBusOption { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual ICollection<PostRegistrationDetailResponse> PostRegistrationDetails { get; set; }
    }
    public  class PostRegistrationDetailResponse
    {
        public int Id { get; set; }
        public int PostRegistrationId { get; set; }
        public int PostId { get; set; }
        public int PositionId { get; set; }

        public string? Note { get; set; }
        public double? SalaryBonus { get; set; }
        public double Salary { get; set; }
        public double FinalSalary { get; set; }

    }
    public class PostRgupdateHistoryResponse
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int PostRegistrationId { get; set; }
        public int? PositionId { get; set; }
        public bool? BusOption { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }

    }

    public class CollaboratorAccountReponse
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? ImgUrl { get; set; }
        public bool PostPermission { get; set; }
        public bool? IsPremium { get; set; }
    }
}
