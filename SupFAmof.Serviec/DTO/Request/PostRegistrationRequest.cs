using SupFAmof.Service.Helpers;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace SupFAmof.Service.DTO.Request
{
    public class PostRegistrationRequest
    {
        [Required(ErrorMessage = "Account ID is required.")]
        public int AccountId { get; set; }
        [JsonIgnore]
        public string? RegistrationCode { get; set; }
        [JsonIgnore]
        public DateTime CreateAt { get; set; }
        [JsonIgnore]
        public int Status { get; set; }

        [Required(ErrorMessage = "School bus option is required.")]
        [ValidateBoolean(ErrorMessage = "Only 'true' or 'false' is allowed.")]
        public bool SchoolBusOption { get; set; }
        public ICollection<PostRegistrationDetailRequest> PostRegistrationDetails { get; set; }
    }

    public class PostRegistrationDetailRequest
    {
        [Required(ErrorMessage = "PostId is required.")]
        public int PostId { get; set; }
        [Required(ErrorMessage = "PositionId is required.")]
        public int PositionId { get; set; }
    }

    public class PostRegistrationUpdateRequest
    {

        [Required(ErrorMessage = "true or false is required.")]
        [ValidateBoolean(ErrorMessage = "Only 'true' or 'false' is allowed.")]
        public bool? SchoolBusOption { get; set; }
        [JsonIgnore]
        public DateTime? CreateAt { get; set; } = DateTime.Now;
        public ICollection<PostRegistrationDetailUpdateRequest> PostRegistrationDetails { get; set; }
    }

    public class PostRegistrationDetailUpdateRequest
    {
        [Required(ErrorMessage = "PositionId is required.")]
        public int PositionId { get; set; }
    }

    public class AproveRequest
    {
        [Required(ErrorMessage = "true or false is required.")]
        [ValidateBoolean(ErrorMessage = "Only 'true' or 'false' is allowed.")]
        public bool IsApproved
        {
            get; set;
        }










    }
}
