using SupFAmof.Service.Helpers;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using static SupFAmof.Service.Utilities.Ultils;

namespace SupFAmof.Service.DTO.Request
{
    public class PostRegistrationRequest
    {
        [JsonIgnore]
        public int Status { get; set; } = 1;
        [JsonIgnore]
        public string RegistrationCode { get; set; } = GenerateRandomCode();
        [JsonIgnore]
        public DateTime CreateAt { get; set; } = GetCurrentDatetime();

        [Required(ErrorMessage = "School bus option is required.")]
        [ValidateBoolean(ErrorMessage = "Only 'true' or 'false' is allowed.")]
        public bool SchoolBusOption { get; set; }
        [Required(ErrorMessage = "PositionId is required.")]
        public int PositionId { get; set; }
    }

   

    public class PostRegistrationUpdateRequest
    {
        public int Id { get; set; }

        [JsonIgnore]
        public DateTime? CreateAt { get; set; } = Utilities.Ultils.GetCurrentDatetime();
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
    public class AproveUpdateIdRequest
    {
        public List<int> Ids
        {
            get; set;
        }

    }
    public class UpdateSchoolBusRequest
    {
        public int Id { get; set; }
        public bool SchoolBusOption { get; set; }
    }
}
