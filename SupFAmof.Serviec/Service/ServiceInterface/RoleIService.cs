using SupFAmof.Service.DTO.Request.Role;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IRoleService
    {
        Task<BaseResponsePagingViewModel<RoleResponse>> GetRoles(RoleResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<RoleResponse>> GetRoleById(int roleId);
        Task<BaseResponseViewModel<RoleResponse>> CreateRole(CreateRoleRequest request);
        Task<BaseResponseViewModel<RoleResponse>> UpdateRole(int roleId, UpdateRoleRequest request);
    }
}
