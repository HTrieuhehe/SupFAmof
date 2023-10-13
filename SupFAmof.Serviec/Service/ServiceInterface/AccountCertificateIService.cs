using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Request.Admission;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IAccountCertificateService
    {
        Task<BaseResponsePagingViewModel<AccountCertificateResponse>> GetAccountCertificates(AccountCertificateResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<AccountCertificateResponse>> GetAccountCertificateById(int accountCertiId);
        Task<BaseResponsePagingViewModel<AccountCertificateResponse>> GetAccountCertificateByAccountId(int accountId, PagingRequest paging);
        Task<BaseResponseViewModel<AccountCertificateResponse>> CreateAccountCertificate(int certificateIssuerId, CreateAccountCertificateRequest request);
        Task<BaseResponseViewModel<AccountCertificateResponse>> UpdateAccountCertificate(int accountId, int accountCertiId, int certificateIssuerId);
    }
}
