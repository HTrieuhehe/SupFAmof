using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SupFAmof.Service.DTO.Request
{
    public class PostRegistrationRequest
    {
        public int AccountId { get; set; }
        public string RegistrationCode { get; set; } = null!;
        public bool? SchoolBusOption { get; set; }
    }

    public class PostRegistrationDetailRequest
    {
        public int PostRegistrationId { get; set; }
        public int PostId { get; set; }
        public int PositionId { get; set; }
    }
}
