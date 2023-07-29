using SupFAmof.Service.DTO.Request.Account;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IAccountService
    {
        Task<BaseResponsePagingViewModel<AccountResponse>> GetAccounts(AccountResponse request, PagingRequest paging);
        Task<BaseResponseViewModel<AccountResponse>> GetAccountById(int accountId);
        Task<BaseResponseViewModel<AccountResponse>> GetAccountByEmail(string email);
        Task<BaseResponseViewModel<AccountResponse>> CreateAccount(CreateAccountRequest request);
        Task<BaseResponseViewModel<AccountResponse>> CreateAccountInformation(int accountId, CreateAccountInformationRequest request);
        Task<BaseResponseViewModel<AccountResponse>> UpdateAccount(int accountId, UpdateAccountRequest request);
        Task Logout(string fcmToken);
        Task<BaseResponseViewModel<LoginResponse>> Login(ExternalAuthRequest data);
    }
}
