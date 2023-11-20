using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Admin;
using SupFAmof.Service.DTO.Request.Staff;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IManageAdmissionProfileService
    {
        Task<BaseResponsePagingViewModel<AdminAccountAdmissionResponse>> GetAdmissionAccount(int adminAccountId, AdminAccountAdmissionResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<AdminAccountAdmissionResponse>> UpdateAdmissionCertificate(int adminAccountId, UpdateAdminAccountAdmissionRequest request);
        Task<BaseResponsePagingViewModel<AdminAccountAdmissionResponse>> SearchAdmissionAccount(int adminAccountId, string search, PagingRequest paging);
    }
}
