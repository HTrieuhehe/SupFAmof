using SupFAmof.Service.DTO.Request.AccounBanking;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IAccountBankingService
    {
        Task<BaseResponseViewModel<AccountBankingResponse>> GetAccountBankingById(int id);
        Task<BaseResponsePagingViewModel<AccountBankingResponse>> GetAccountBankings(AccountBankingResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<AccountBankingResponse>> CreateAccountBanking(int accountId, CreateAccountBankingRequest request);
        Task<BaseResponseViewModel<AccountBankingResponse>> UpdateAccountBanking(int accountId, int accountBankingId, UpdateAccountBankingRequest request);
    }
}
