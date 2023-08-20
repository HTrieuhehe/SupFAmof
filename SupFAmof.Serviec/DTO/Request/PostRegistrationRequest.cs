using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SupFAmof.Service.DTO.Request
{
    public class PostRegistrationRequest
    {
        public int AccountId { get; set; }
        public string RegistrationCode { get; set; } = null!;
        [JsonIgnore]
        public DateTime CreateAt { get; set; } 
        public bool? SchoolBusOption { get; set; }
        public ICollection<PostRegistrationDetailRequest> PostRegistrationDetails { get; set; }
    }

    public class PostRegistrationDetailRequest
    {
        public int PostId { get; set; }
        public int PositionId { get; set; }
    }

    public class PostRegistrationUpdateRequest
    {
        public bool? SchoolBusOption { get; set; }
        [JsonIgnore]
        public DateTime? UpdateAt { get; set; } = DateTime.Now;
        public ICollection<PostRegistrationDetailUpdateRequest> PostRegistrationDetails { get; set; }
    }

    public class PostRegistrationDetailUpdateRequest
    {
        public int PositionId { get; set; }
    }
    public class PostRegistrationUpdateBookingRequest
    {
        public bool? SchoolBusOption { get; set; }
        [JsonIgnore]
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public ICollection<PostRegistrationDetailUpdateBookingRequest> PostRegistrationDetails { get; set; }
    }

    public class PostRegistrationDetailUpdateBookingRequest
    {
        public int PositionId { get; set; }
    }




}
