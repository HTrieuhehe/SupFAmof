﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IAttendanceService
    {
        #region Admission View Attendance History

        Task<BaseResponsePagingViewModel<AdmissionAttendanceResponse>> GetAttendanceHistoryByPositionId(int accountId, int positionId, PagingRequest paging);

        #endregion
        Task<BaseResponseViewModel<dynamic>> AdmissionConfirmAttendance(int accountId, int positionId, List<AdmissionConfirmAttendanceRequest> requests);

        #region Collaborator View Attendance History

        #endregion

    }
}
