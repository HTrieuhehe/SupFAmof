﻿using SupFAmof.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Response
{
    public class ContractResponse
    {
        public int? Id { get; set; }
        public int? CreatePersonId { get; set; }
        public string? ContractName { get; set; }
        public string? ContractDescription { get; set; }
        public string? SampleFile { get; set; }
        public DateTime? SigningDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? TotalSalary { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual AccountResponse? CreatePerson { get; set; }
    }
}
