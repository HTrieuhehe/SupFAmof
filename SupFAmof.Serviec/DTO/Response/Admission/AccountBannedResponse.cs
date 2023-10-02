using SupFAmof.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Response.Admission
{
    public class AccountBannedResponse
    {
        public int Id { get; set; }
        public int AccountIdBanned { get; set; }
        public int BannedPersonId { get; set; }
        public DateTime DayStart { get; set; }
        public DateTime DayEnd { get; set; }
        public string? Note { get; set; }
        public bool IsActive { get; set; }

        public virtual AccountResponse? BannedPerson { get; set; }
    }
}
