using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using SupFAmof.Service.DTO.Request;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface ICheckInService
    {
        Task CheckIn(CheckInRequest checkin);
    }
}
