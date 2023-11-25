using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Admission;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IContractService
    {
        Task<BaseResponsePagingViewModel<AdmissionApplicationResponse>> GetAdmissionContracts(int accountId, AdmissionApplicationResponse filter, PagingRequest paging);
        Task<BaseResponsePagingViewModel<AdmissionApplicationResponse>> AdmisionSearchContract(int accountId, string search, PagingRequest paging);
        Task<BaseResponseViewModel<AdmissionApplicationResponse>> GetAdmissionContractById(int accountId, int contractId);
        Task<BaseResponseViewModel<AdmissionApplicationResponse>> CreateAdmissionContract(int accountId, CreateAdmissionContractRequest request);
        Task<BaseResponseViewModel<AdmissionApplicationResponse>> UpdateAdmissionContract(int accountId, int contractId, UpdateAdmissionContractRequest request);
        Task<BaseResponseViewModel<AdmissionApplicationResponse>> DisableAdmissionContract(int accountId, int contractId);
        Task<BaseResponseViewModel<bool>> AdmissionSendContractEmail(int accountId, int contractId, List<int> collaboratorAccountId);

        //Collabotator Contract Zone
        Task<BaseResponsePagingViewModel<AccountContractResponse>> GetContracts(AccountContractResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<AccountContractResponse>> GetContractById(int contractId);
        Task<BaseResponsePagingViewModel<AccountContractResponse>> GetContractsByAccountId(int accountId, PagingRequest paging);
        Task<BaseResponseViewModel<AccountContractResponse>> ConfirmContract(int accountId, int accountContractId, int status);
    }
}
