using SupFAmof.Data.Entity;
using SupFAmof.Service.DTO.Response.Admission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Response
{
    public class PostResponse
    {
        public int Id { get; set; }
        public string? PostCode { get; set; }
        public string? PostDescription { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public TimeSpan TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public int? Priority { get; set; }
        public bool IsPremium { get; set; }
        public string? Location { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual AccountResponse? Account { get; set; }
        public virtual PostTitleResponse? PostTitle { get; set; }
        public virtual ICollection<PostPositionResponse>? PostPositions { get; set; }
        public virtual ICollection<TrainingPositionResponse>? TrainingPositions { get; set; }


        //public virtual ICollection<PostRegistrationDetail> PostRegistrationDetails { get; set; }
        //public virtual ICollection<PostTgupdateHistory> PostTgupdateHistories { get; set; }
        //public virtual ICollection<TrainingPosition> TrainingPositions { get; set; }
    }
}
