using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SupFAmof.Service.DTO.Response
{
    public class CheckAttendanceResponse
    {
        public int? Id { get; set; }
        public int? PostRegistrationId { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public DateTime? ConfirmTime { get; set; }
        public int? Status { get; set; }
        public string? Note { get; set; }
    }

    public class CheckAttendancePostResponse
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
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public virtual ICollection<CheckAttendanceAdmission> Records { get; set; }

    }
    public class CheckAttendanceAdmission
    {
        public int? Id { get; set; }
        public int? PostRegistrationId { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public DateTime? ConfirmTime { get; set; }
        public int? Status { get; set; }
        public string? Note { get; set; }
        public virtual ManageCollabAccountResponse? Account { get; set; }
    }
}
