using SupFAmof.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Response.Admission
{
    public class AdmissionPostResponse
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int PostTitleId { get; set; }
        public int PostCode { get; set; }
        public string? PostDescription { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public TimeSpan TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public int? Priority { get; set; }
        public bool IsPremium { get; set; }
        public string? Location { get; set; }
        public bool AttendanceComplete { get; set; }
        public bool IsActive { get; set; }
        public bool IsEnd { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public AccountResponse? Account { get; set; }
        public PostTitleResponse? PostTitle { get; set; }
        public ICollection<PostPositionResponse>? PostPositions { get; set; }
        //public virtual ICollection<PostRegistrationDetail> PostRegistrationDetails { get; set; }
        //public virtual ICollection<PostTgupdateHistory> PostTgupdateHistories { get; set; }
        public ICollection<TrainingPositionResponse>? TrainingPositions { get; set; }
    }
}
