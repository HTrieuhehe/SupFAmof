using SupFAmof.Service.DTO.Request.Staff;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admin;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IAdminAccountService
    {
        Task<BaseResponsePagingViewModel<AdminAccountResponse>> GetAdmins(AdminAccountResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<AdminAccountResponse>> GetAdminById(int adminId);
        Task<BaseResponseViewModel<dynamic>> Login(LoginRequest request);
        Task<BaseResponseViewModel<AdminAccountResponse>> CreateAdminManager(CreateAdminAccountRequest request);
        Task<BaseResponseViewModel<AdminAccountResponse>> UpdateAdmin(int adminId, UpdateAdminAccountRequest request);
    }
}
