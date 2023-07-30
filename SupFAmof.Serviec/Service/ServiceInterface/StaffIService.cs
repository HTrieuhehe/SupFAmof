using SupFAmof.Service.DTO.Request.Staff;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IStaffService
    {
        Task<BaseResponsePagingViewModel<StaffResponse>> GetStaffs(StaffResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<StaffResponse>> GetStaffById(int staffId);
        Task<BaseResponseViewModel<dynamic>> Login(LoginRequest request);
        Task<BaseResponseViewModel<StaffResponse>> CreateAdminManager(CreateStaffRequest request);
        Task<BaseResponseViewModel<StaffResponse>> UpdateStaff(int staffId, UpdateStaffRequest request);
    }
}
