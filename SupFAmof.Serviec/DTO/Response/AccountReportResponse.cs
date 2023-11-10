using System;
using System.Linq;
using System.Text;
using SupFAmof.Data.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SupFAmof.Service.DTO.Response
{
    public class AccountReportResponse
    {
        public int? Id { get; set; }
        public int? AccountId { get; set; }
        public int? PositionId { get; set; }
        public double? Salary { get; set; }
        public DateTime? CreateAt { get; set; }
    }

    public class ReportPostRegistrationResponse
    {
        public int? Id { get; set; }
        public string? RegistrationCode { get; set; }
        public int PositionId { get; set; }
        public string? Note { get; set; }
        public double Salary { get; set; }
        public int? Status { get; set; }
        public bool? SchoolBusOption { get; set; }
        public string? CreateAt { get; set; }
        public string? UpdateAt { get; set; }

        public virtual ReportPostPositionResponse? Position { get; set; }
    }

    public class ReportPostResponse
    {
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

        public ICollection<ReportPostPositionResponse>? PostPositions { get; set; }
    }

    public class ReportPostPositionResponse
    {
        public int? Id { get; set; }
        public int? PostId { get; set; }
        public int? TrainingCertificateId { get; set; }
        public int? DocumentId { get; set; }
        public string? PositionName { get; set; }
        public string? PositionDescription { get; set; }
        public string? SchoolName { get; set; }
        public string? Location { get; set; }
        public string? Latitude { get; set; }
        public string? Longtitude { get; set; }
        public TimeSpan TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public int? Status { get; set; }
        public bool? IsBusService { get; set; }
        public int? Amount { get; set; }
        public double? Salary { get; set; }
    }
}
