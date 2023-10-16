using SupFAmof.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Response
{
    public class NotificationHistoryResponse
    {
        public int Id { get; set; }
        public int RecipientId { get; set; }
        public string? Title { get; set; }
        public string? Text { get; set; }
        public int Status { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual AccountResponse? Recipient { get; set; }
    }
}
