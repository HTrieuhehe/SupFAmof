using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.DTO.Response
{
    public class LoginResponse
    {
        public string? access_token { get; set; }
        public AccountResponse? account { get; set; }
    }
}
