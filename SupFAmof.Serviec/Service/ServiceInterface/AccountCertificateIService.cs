using SupFAmof.Service.DTO.Request.Admission;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response.Admission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupFAmof.Service.DTO.Response;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IAccountCertificateService
    {
        Task<BaseResponsePagingViewModel<AccountCertificateResponse>> GetAccountCertificates(AccountCertificateResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<AccountCertificateResponse>> GetAccountCertificateById(int accountCertiId);
        Task<BaseResponsePagingViewModel<AccountCertificateResponse>> GetAccountCertificateByAccountId(int accountId, PagingRequest paging);
        Task<BaseResponseViewModel<AccountCertificateResponse>> CreateAccountCertificate(int createPersonId, CreateAccountCertificateRequest request);
        Task<BaseResponseViewModel<AccountCertificateResponse>> UpdateAccountCertificate(int accountId, int accountCertiId, int createPersonId);
    }
}
