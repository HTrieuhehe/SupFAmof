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
        Task<BaseResponsePagingViewModel<AdmissionContractResponse>> GetAdmissionContracts(int accountId, AdmissionContractResponse filter, PagingRequest paging);
        Task<BaseResponsePagingViewModel<AdmissionContractResponse>> AdmisionSearchContract(int accountId, string search, PagingRequest paging);
        Task<BaseResponseViewModel<AdmissionContractResponse>> GetAdmissionContractById(int accountId, int contractId);
        Task<BaseResponseViewModel<AdmissionContractResponse>> CreateAdmissionContract(int accountId, CreateAdmissionContractRequest request);
        Task<BaseResponseViewModel<AdmissionContractResponse>> UpdateAdmissionContract(int accountId, int contractId, UpdateAdmissionContractRequest request);
        Task<BaseResponseViewModel<AdmissionContractResponse>> DisableAdmissionContract(int accountId, int contractId);
        Task<BaseResponseViewModel<bool>> AdmissionSendContractEmail(int accountId, int contractId, List<int> collaboratorAccountId);

        Task<BaseResponsePagingViewModel<ContractResponse>> GetContracts(ContractResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<ContractResponse>> GetContractsById(int contractId);
    }
}
