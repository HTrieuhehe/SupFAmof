using static SupFAmof.Service.Utilities.Ultils;

namespace SupFAmof.Service.DTO.Request
{
    public class PostAttendeeRequest
    {
        public int AccountId { get; set; }
        public int? PositionId { get; set; }
        public int? TrainingPositionId { get; set; }
        public DateTime ConfirmAt { get; set; } = GetCurrentTime();
    }
}
