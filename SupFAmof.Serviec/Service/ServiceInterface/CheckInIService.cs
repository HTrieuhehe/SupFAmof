using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface ICheckInService
    {
        Task<BaseResponseViewModel<dynamic>> CheckIn(int accountId, CheckInRequest checkin);
        Task<BaseResponseViewModel<dynamic>> CheckOut(int accountId, CheckOutRequest request);
        Task<byte[]> QrGenerate(QrRequest request);

    }
}
