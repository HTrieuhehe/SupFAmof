using System.Text.Json.Serialization;

namespace SupFAmof.Service.DTO.Request
{
    public class PostRegistrationRequest
    {
        public int AccountId { get; set; }
        [JsonIgnore]
        public string? RegistrationCode { get; set; } 
        [JsonIgnore]
        public DateTime CreateAt { get; set; }
        [JsonIgnore]
        public int Status { get; set; }
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
        public DateTime? CreateAt { get; set; } = DateTime.Now;
        public ICollection<PostRegistrationDetailUpdateRequest> PostRegistrationDetails { get; set; }
    }

    public class PostRegistrationDetailUpdateRequest
    {
        public int PositionId { get; set; }
    }

   








}
