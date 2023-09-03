using SupFAmof.Service.Service.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.Service
{
    public class TimezoneService : ITimezoneService
    {
        private readonly TimeZoneInfo hoChiMinhTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        public DateTime GetCurrentTime()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, hoChiMinhTimeZone);
        }
    }
}
