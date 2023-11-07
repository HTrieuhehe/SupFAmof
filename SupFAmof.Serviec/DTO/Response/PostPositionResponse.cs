using SupFAmof.Data.Entity;
using SupFAmof.Service.DTO.Response.Admission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Response
{
    public class PostPositionResponse
    {
        public PostPositionResponse()
        {
            RegisterAmount = 0;
        }

        public int Id { get; set; }
        public int? PostId { get; set; }
        public int? TrainingCertificateId { get; set; }
        public int? DocumentId { get; set; }
        public string? PositionName { get; set; }
        public string? PositionDescription { get; set; }
        public string? SchoolName { get; set; }
        public string? Location { get; set; }
        public DateTime? Date { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public TimeSpan TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
        public int? Status { get; set; }
        public bool? IsBusService { get; set; }
        public int? Amount { get; set; }
        public double? Salary { get; set; }

        public int? RegisterAmount { get; set; }

    }
}
