using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.Helpers
{
    public class Token
    {
        public string Issuer { get; set; }
        public string SecretKey { get; set; }
        public string PrivateKey { get; set; }
    }
}
