using AutoMapper;
using AutoMapper.QueryableExtensions;
using NTQ.Sdk.Core.Utilities;
using Service.Commons;
using SupFAmof.Data.Entity;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;
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

        public async Task<BaseResponsePagingViewModel<AccountReportProblemResponse>> GetAccountReportProblemsByToken(int accountId, AccountReportProblemResponse filter, PagingRequest paging)
        {
            try
            {
                var reportProblem = _unitOfWork.Repository<AccountReportProblem>().GetAll()
                                               .Where(x => x.AccountId == accountId)
                                               .ProjectTo<AccountReportProblemResponse>(_mapper.ConfigurationProvider)
                                                .DynamicFilter(filter)
                                                .DynamicSort(filter)
                                                .PagingQueryable(paging.Page, paging.PageSize,
                                                           Constants.LimitPaging, Constants.DefaultPaging);

                return new BaseResponsePagingViewModel<AccountReportProblemResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = reportProblem.Item1
                    },
                    Data = reportProblem.Item2.ToList(),
                };

            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<AdmissionAccountReportProblemResponse>> GetAdmissionAccountReportProblemsByToken(int accountId, AdmissionAccountReportProblemResponse filter, PagingRequest paging)
        {
            try
            {
                //check admission permission

                var checkAdmission = _unitOfWork.Repository<Account>().Find(x => x.Id == accountId && x.RoleId == (int)SystemRoleEnum.AdmissionManager);

                if (checkAdmission == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                var reportProblem = _unitOfWork.Repository<AccountReportProblem>().GetAll()
                                               .ProjectTo<AdmissionAccountReportProblemResponse>(_mapper.ConfigurationProvider)
                                                .DynamicFilter(filter)
                                                .DynamicSort(filter)
                                                .PagingQueryable(paging.Page, paging.PageSize,
                                                           Constants.LimitPaging, Constants.DefaultPaging);

                return new BaseResponsePagingViewModel<AdmissionAccountReportProblemResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = reportProblem.Item1
                    },
                    Data = reportProblem.Item2.ToList(),
                };

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<BaseResponseViewModel<AdmissionAccountReportProblemResponse>> RejectReportProblem(int accountId, UpdateAdmissionAccountReportProblemRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponseViewModel<AdmissionAccountReportProblemResponse>> ApproveReportProblem(int accountId, UpdateAdmissionAccountReportProblemRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
