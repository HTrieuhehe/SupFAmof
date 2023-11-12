using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IAttendanceService
    {
        #region Admission View Attendance History

        Task<BaseResponsePagingViewModel<AdmissionAttendanceResponse>> GetAttendanceHistoryByPositionId(int accountId, int positionId, PagingRequest paging);

        #endregion

        #region Collaborator View Attendance History

        #endregion

    }
}
