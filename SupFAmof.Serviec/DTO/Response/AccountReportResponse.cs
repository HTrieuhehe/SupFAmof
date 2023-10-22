using SupFAmof.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Response
{
    public class AccountReportResponse
    {
        public int? Id { get; set; }
        public int? AccountId { get; set; }
        public int? PostId { get; set; }
        public double? Salary { get; set; }
        public DateTime? Date { get; set; }

        public virtual PostResponse? Post { get; set; }
    }

    public class AccountReportPostResponse
    {
        public int? Id { get; set; }
        public int? AccountId { get; set; }
        public int? PostCategoryId { get; set; }
        public string? PostCode { get; set; }
        public string? PostDescription { get; set; }
        public string? PostImg { get; set; }
        public int? Priority { get; set; }
        public string? DateFrom { get; set; }
        public string? DateTo { get; set; }
        public bool? IsPremium { get; set; }
        public int? Status { get; set; }
        public bool? AttendanceComplete { get; set; }
        public string? CreateAt { get; set; }
        public string? UpdateAt { get; set; }
    }

    public class AccountReportPostPositionResponse
    {
        public int? Id { get; set; }
        public int? PostId { get; set; }
        public int? TrainingCertificateId { get; set; }
        public int? DocumentId { get; set; }
        public string? PositionName { get; set; }
        public string? PositionDescription { get; set; }
        public string? SchoolName { get; set; }
        public string? Location { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longtitude { get; set; }
        public string? TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public bool? IsBusService { get; set; }
        public int Status { get; set; }
        public int Amount { get; set; }
        public double Salary { get; set; }
    }
}
