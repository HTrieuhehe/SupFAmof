using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Request.Admission
{
    public class CreatePostRequest
    {
        //public int Id { get; set; }
        //public int AccountId { get; set; }
        public int PostTitleId { get; set; }
        //public int PostCode { get; set; }
        public string? PostDescription { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public TimeSpan TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public int? Priority { get; set; }
        public bool IsPremium { get; set; }
        public string? Location { get; set; }
        //public bool AttendanceComplete { get; set; }
        //public bool IsActive { get; set; }
        //public bool IsEnd { get; set; }
        //public DateTime CreateAt { get; set; }
        //public DateTime? UpdateAt { get; set; }

        public List<CreatePostPositionRequest>? PostPositions { get; set; }
        public List<CreatePostTrainingPositionRequest>? TrainingPositions { get; set;}
    }

    public class CreatePostPositionRequest
    {
        //public int Id { get; set; }
        //public int PostId { get; set; }
        public string? PositionName { get; set; }
        public int Amount { get; set; }
        public double Salary { get; set; }
    }

    public class CreatePostTrainingPositionRequest
    {
        //public int Id { get; set; }
        //public int PostId { get; set; }
        public string? PositionName { get; set; }
        public int Amount { get; set; }
        public double? Salary { get; set; }
    }

    public class UpdatePostRequest
    {

    }

    public class UpdatePostPositionRequest
    {

    }

    public class UpdatePostTrainingPositionRequest
    {

    }
}
