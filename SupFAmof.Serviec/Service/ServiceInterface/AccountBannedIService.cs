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
    public interface IAccountBannedService
    {
        Task<BaseResponsePagingViewModel<AccountBannedResponse>> GetAccountBanneds(AccountBannedResponse filter, PagingRequest paging);
        Task<BaseResponsePagingViewModel<AccountBannedResponse>> GetAccountBannedByToken(int accountId, PagingRequest paging);
        Task<BaseResponseViewModel<AccountBannedResponse>> CreateAccountBanned(int accountId, CreateAccountBannedRequest request);
        Task<BaseResponseViewModel<AccountBannedResponse>> UpdateAccountBanned(int accountId, int accountBannedId, UpdateAccountBannedRequest request);
    }
}
