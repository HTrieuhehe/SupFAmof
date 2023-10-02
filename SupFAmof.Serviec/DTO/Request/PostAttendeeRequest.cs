using System.ComponentModel.DataAnnotations;
using static ServiceStack.LicenseUtils;
using static SupFAmof.Service.Utilities.Ultils;

namespace SupFAmof.Service.DTO.Request
{
    public class PostAttendeeRequest
    {
        [Required(ErrorMessage = "AccountId is required.")]
        public int AccountId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "PositionId must be greater than 0.")]
        public int? PositionId { get; set; }

        [Required(ErrorMessage = "PostId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "PostId must be greater than 0.")]
        public int PostId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "TrainingPositionId must be greater than 0.")]
        public int? TrainingPositionId { get; set; }

        [Required(ErrorMessage = "ConfirmAt is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "ConfirmAt must be a valid DateTime.")]
        public DateTime ConfirmAt { get; set; } = GetCurrentTime();
    }
}
