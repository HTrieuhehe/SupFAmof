using System;
using System.Collections.Generic;

namespace SupFAmof.Data.Entity
{
    public partial class NotificationHistory
    {
        public int Id { get; set; }
        public int RecipientId { get; set; }
        public string Title { get; set; } = null!;
        public string? Text { get; set; }
        public int NotificationType { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual Account Recipient { get; set; } = null!;
    }
}
