using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.Exceptions
{
    public class MapServiceException :Exception
    {
        public String Code { get; set; }

        public MapServiceException()
        {
        }


        public MapServiceException(string message) : base(message)
        {
        }


        public MapServiceException(string message, string code) : base(message)
        {
            Code = code;
        }

        public MapServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
