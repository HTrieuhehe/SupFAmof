﻿using SupFAmof.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Response.Admission
{
    public class AdmissionAccountContractResponse
    {
        public int? Id { get; set; }
        public string? ContractName { get; set; }
        public int? CreatePersonId { get; set; }
        public string? ContractDescription { get; set; }
        public string? SampleFile { get; set; }
        public DateTime? SigningDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double? TotalSalary { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual ICollection<AccountContractResponse>? AccountContracts { get; set; }
    }

    public class AdmissionAccountContractCompleteResponse
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int AccountId { get; set; }
        public byte[]? SubmittedBinaryFile { get; set; }
        public string? SubmittedFile { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
