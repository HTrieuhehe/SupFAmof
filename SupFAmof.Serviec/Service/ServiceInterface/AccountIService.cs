using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Request.Account;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.DTO.Request.Admission.AccountRequest;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IAccountService
    {
        Task<BaseResponsePagingViewModel<AccountResponse>> GetAccounts(AccountResponse request, PagingRequest paging);
        Task<BaseResponseViewModel<AccountResponse>> GetAccountById(int accountId);
        Task<BaseResponsePagingViewModel<AccountResponse>> SearchCollaboratorByEmail(string email, PagingRequest paging);
        Task<BaseResponseViewModel<AccountResponse>> GetAccountByEmail(string email);
        Task<BaseResponseViewModel<AccountResponse>> CreateAccount(CreateAccountRequest request);
        Task<BaseResponseViewModel<AccountResponse>> CreateAccountInformation(int accountId, CreateAccountInformationRequest request);
        Task<BaseResponseViewModel<AccountResponse>> UpdateAccount(int accountId, UpdateAccountRequest request);
        Task<BaseResponseViewModel<AdmissionAccountResponse>> UpdateAdmissionAccount(int accountId, UpdateAdmissionAccountRequest request);
        Task<BaseResponseViewModel<AdmissionAccountResponse>> UpdateAdmissionAccountAvatart(int accountId, UpdateAccountAvatar request);
        Task<BaseResponseViewModel<AccountResponse>> DisableAccount(int accountId);
        Task Logout(string expoToken, int accountId, int status);
        Task<BaseResponseViewModel<LoginResponse>> AdmissionLogin(ExternalAuthRequest data);
        Task<BaseResponseViewModel<dynamic>> InputVerifycationCode(int accountId, int code, int roleId);
        Task<BaseResponseViewModel<LoginResponse>> Login(ExternalAuthRequest data);
        Task<BaseResponseViewModel<AccountReactivationResponse>> EnableAccount(int accountId);
        Task<BaseResponseViewModel<AdmissionAccountResponse>> GetAccountAdmissionById(int accountId);
        Task<BaseResponseViewModel<AccountResponse>> UpdateAccountAvatar(int accountId, UpdateAccountAvatar request);
        Task<BaseResponseViewModel<TotalAccountResponse>> ViewCollaborator();
        Task<BaseResponsePagingViewModel<ManageCollabAccountResponse>> GetAllCollabAccount(int accountId,PagingRequest paging);
    }
}
