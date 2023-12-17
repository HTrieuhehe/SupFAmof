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
    public interface IAdmissionCredentialService
    {
        Task<BaseResponseViewModel<AdmissionAccountResponse>> CreateAdmissionCredential(int administratorId, int accountId);
        Task<BaseResponseViewModel<AdmissionAccountResponse>> DisableAdmissionCredential(int administratorId, int accountId);
        Task<BaseResponsePagingViewModel<AdmissionAccountResponse>> GetAdmissionProfile(int administratorId, AdmissionAccountResponse filter, PagingRequest paging);
    }
}
