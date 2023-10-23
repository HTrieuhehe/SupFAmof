using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Request
{
    public class ExternalAuthRequest
    {
        public string? IdToken { get; set; }
        public string ExpoPushToken { get; set; } = "";
    }
}
