using AutoMapper;
using NTQ.Sdk.Core.Utilities;
using SupFAmof.Data.Entity;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Service.ServiceInterface;
using SupFAmof.Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SupFAmof.Service.Helpers.Enum;
using static SupFAmof.Service.Helpers.ErrorEnum;

namespace SupFAmof.Service.Service
{
    public class AccountReportProblemService : IAccountReportProblemService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AccountReportProblemService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponseViewModel<AccountReportProblemResponse>> CreateAccountReportProblem(int accountId, CreateAccountReportProblemRequest request)
        {
            try
            {
                var checkAccountCollab =
                    await _unitOfWork.Repository<Account>()
                                     .FindAsync(x => x.Id == accountId && x.RoleId == (int)SystemRoleEnum.Collaborator);

                if (checkAccountCollab == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                var report = _mapper.Map<CreateAccountReportProblemRequest, AccountReportProblem>(request);

                report.AccountId = accountId;
                report.ReportDate = Ultils.GetCurrentDatetime();
                report.Status = (int)ReportProblemStatusEnum.Pending;

                await _unitOfWork.Repository<AccountReportProblem>().InsertAsync(report);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AccountReportProblemResponse>
                {
                    Status = new StatusViewModel
                    {
                        Message = "Success",
                        ErrorCode = 0,
                        Success = true,
                    },
                    Data = _mapper.Map<AccountReportProblemResponse>(report)

                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<BaseResponsePagingViewModel<AccountReportProblemResponse>> GetAccountReportProblemsByToken(int accountId, AccountReportProblemResponse filter, PagingRequest paging)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponsePagingViewModel<AccountReportProblemResponse>> GetAdmissionAccountReportProblemsByToken(int accountId, AccountReportProblemResponse filter, PagingRequest paging)
        {
            throw new NotImplementedException();
        }
    }
}
