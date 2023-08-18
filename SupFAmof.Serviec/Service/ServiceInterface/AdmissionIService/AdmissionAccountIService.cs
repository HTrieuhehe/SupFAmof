using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Account;
using SupFAmof.Service.DTO.Request.Admission.AccountRequest;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.Service.ServiceInterface.AdmissionIService
{
    public interface IAdmissionAccountService
    {
        Task<BaseResponsePagingViewModel<AdmissionAccountResponse>> GetAccounts(AdmissionAccountResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<AdmissionAccountResponse>> GetAccountById(int accountId);
        Task<BaseResponseViewModel<AdmissionAccountResponse>> GetAccountByEmail(string email);
        Task<BaseResponseViewModel<AdmissionAccountResponse>> UpdateAccount(int accountId, UpdateAdmissionAccountRequest request);
        Task<BaseResponseViewModel<AdmissionAccountResponse>> DisableAccount(int accountId);
        Task<BaseResponseViewModel<AdmissionAccountResponse>> CreateAccount(CreateAccountRequest request);
        Task Logout(string fcmToken);
        Task<BaseResponseViewModel<LoginResponse>> Login(ExternalAuthRequest data);
    }
}
