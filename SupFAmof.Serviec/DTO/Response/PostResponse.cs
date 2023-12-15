using System;
using System.Linq;
using System.Text;
using SupFAmof.Data.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using SupFAmof.Service.DTO.Response.Admission;

namespace SupFAmof.Service.DTO.Response
{
    public class PostResponse
    {
        public PostResponse()
        {
            RegisterAmount = 0;
            TotalAmountPosition = 0;
            PendingRegisterAmount = 0;
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
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        //The ?? operator is used to provide a default value in case the left-hand operand

        public int? PendingRegisterAmount;

        public int? RegisterAmount;
        //public  int? RegisterAmount { get => registerAmount ?? 0; set => registerAmount = value; }

        public int? TotalAmountPosition;
        //public  int? TotalAmountPosition { get => totalAmountPosition ?? 0; set => totalAmountPosition = value; }

        public string? TimeFrom { get; set; }
        public string? TimeTo { get; set; }

        public AccountResponse? Account { get; set; }
        public PostCategoryResponse? PostCategory { get; set; }
        public ICollection<PostPositionResponse>? PostPositions { get; set; }
    }

    public class TimeFromFilter
    {
        public DateTime? DateFromStart { get; set; }
        public DateTime? DateFromEnd { get; set; }
        public DateTime? CreateAtStart { get; set; }
        public DateTime? CreateAtEnd { get; set; }
    }
}
