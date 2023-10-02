using System.Text.Json.Serialization;
using static ServiceStack.LicenseUtils;
using System.ComponentModel.DataAnnotations;
using static SupFAmof.Service.Utilities.Ultils;

namespace SupFAmof.Service.DTO.Request
{
    public class DocumentRequest
    {
        [Required(ErrorMessage = "DocName is required.")]
        public string? DocName { get; set; }

        [Required(ErrorMessage = "DocUrl is required.")]
        [Url(ErrorMessage = "DocUrl must be a valid URL.")]
        public string? DocUrl { get; set; }
        [JsonIgnore]
        public DateTime CreateAt { get; set; } = GetCurrentTime();


    }
    public class DocumentUpdateRequest
    {
        public string? DocName { get; set; }

        public string? DocUrl { get; set; }
        [JsonIgnore]
        public DateTime CreateAt { get; set; } = GetCurrentTime();

    }
    public class DocumentDisableRequest
    {
        public bool? IsActive { get; set; }

    }
}
