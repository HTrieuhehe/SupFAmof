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
        Task<BaseResponsePagingViewModel<AdmissionAccountContractResponse>> GetAdmissionContracts(int accountId, AdmissionAccountContractResponse filter, PagingRequest paging);
        Task<BaseResponsePagingViewModel<AdmissionAccountContractResponse>> AdmisionSearchContract(int accountId, string search, PagingRequest paging);
        Task<BaseResponseViewModel<AdmissionAccountContractResponse>> GetAdmissionContractById(int accountId, int contractId);
        Task<BaseResponseViewModel<AdmissionAccountContractResponse>> CreateAdmissionContract(int accountId, CreateAdmissionContractRequest request);
        Task<BaseResponseViewModel<AdmissionAccountContractResponse>> UpdateAdmissionContract(int accountId, int contractId, UpdateAdmissionContractRequest request);
        Task<BaseResponseViewModel<AdmissionAccountContractResponse>> DisableAdmissionContract(int accountId, int contractId);
        Task<BaseResponseViewModel<bool>> AdmissionSendContractEmail(int accountId, int contractId, List<int> collaboratorAccountId);
        Task<BaseResponseViewModel<AdmissionAccountContractResponse>> AdmissionCompleteContract(int accountId, int accountContractId);

        //Collabotator Contract Zone
        Task<BaseResponsePagingViewModel<AccountContractResponse>> GetContracts(AccountContractResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<AccountContractResponse>> GetContractById(int contractId);
        Task<BaseResponsePagingViewModel<AccountContractResponse>> GetContractsByAccountId(int accountId, PagingRequest paging);
        Task<BaseResponseViewModel<AccountContractResponse>> ConfirmContract(int accountId, int accountContractId, int status);
    }
}
