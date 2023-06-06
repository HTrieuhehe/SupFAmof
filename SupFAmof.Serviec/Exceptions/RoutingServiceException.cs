using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.Exceptions
{
    public class RoutingServiceException : Exception
    {
        public RoutingServiceException()
        {
        }

        public RoutingServiceException(string message) : base(message)
        {
        }

        public RoutingServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
