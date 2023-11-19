using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Admin;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface ISystemManagementService
    {
        Task<BaseResponsePagingViewModel<AdminSystemManagementResponse>> GetRoles(int accountId, AdminSystemManagementResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<AdminSystemManagementResponse>> GetRoleById(int accountId, int roleId);
        Task<BaseResponseViewModel<AdminSystemManagementResponse>> CreateRole(int accountId, CreateAdminSystemManagementRequest request);
        Task<BaseResponseViewModel<AdminSystemManagementResponse>> UpdateRole(int accountId, int roleId, UpdateAdminSystemManagementRequest request);
        Task<BaseResponseViewModel<AdminSystemManagementResponse>> DisableRole(int accountId, int roleId);
    }
}
