using System;
using AutoMapper;
using System.Linq;
using System.Text;
using Service.Commons;
using StackExchange.Redis;
using SupFAmof.Data.Entity;
using LAK.Sdk.Core.Utilities;
using System.Threading.Tasks;
using SupFAmof.Data.UnitOfWork;
using System.Security.Principal;
using SupFAmof.Service.Utilities;
using System.Collections.Generic;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using Microsoft.EntityFrameworkCore;
using SupFAmof.Service.DTO.Response;
using AutoMapper.QueryableExtensions;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.ErrorEnum;

namespace SupFAmof.Service.Service
{
    public class AccountReportService : IAccountReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountReportService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponsePagingViewModel<AccountReportResponse>> GetAccountReportByToken
            (int accountId, AccountReportResponse filter, AccountReportFilter dateFilter, PagingRequest paging)
        {
            try
            {
                var accountReport = _unitOfWork.Repository<AccountReport>()
                                                .GetAll()
                                                .ProjectTo<AccountReportResponse>(_mapper.ConfigurationProvider)
                                                .Where(x => x.AccountId == accountId)
                                                .OrderByDescending(x => x.CreateAt)
                                                .DynamicFilter(filter)
                                                .DynamicSort(paging.Sort, paging.Order);
                                                
                var dateFilterLists = FilterReportDate(accountReport, dateFilter).PagingQueryable(paging.Page, paging.PageSize);

                return new BaseResponsePagingViewModel<AccountReportResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = dateFilterLists.Item1
                    },
                    Data = dateFilterLists.Item2.ToList()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<AccountReportResponse>> GetAccountReports(AccountReportResponse filter, PagingRequest paging)
        {
            try
            {
                var accountReport = _unitOfWork.Repository<AccountReport>()
                                                .GetAll()
                                                .ProjectTo<AccountReportResponse>(_mapper.ConfigurationProvider)
                                                .DynamicFilter(filter)
                                                .DynamicSort(paging.Sort, paging.Order)
                                                .OrderByDescending(x => x.CreateAt)
                                                .PagingQueryable(paging.Page, paging.PageSize);

                return new BaseResponsePagingViewModel<AccountReportResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = accountReport.Item1
                    },
                    Data = accountReport.Item2.ToList()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<ReportPostRegistrationResponse>> GetReportRegistrationById(int accountId, int accountReportId)
        {
            try
            {
                var accountReport = await _unitOfWork.Repository<AccountReport>().FindAsync(x => x.Id == accountReportId && x.AccountId == accountId);

                if (accountReport == null)
                {
                    throw new ErrorResponse(404, (int)AccountReportErrorEnum.NOT_FOUND_REPORT,
                                                    AccountReportErrorEnum.NOT_FOUND_REPORT.GetDisplayName());
                }

                //var postRegistration = await _unitOfWork.Repository<PostRegistration>()
                //                                    .FindAsync(x => x.PostRegistrationDetails
                //                                    .Any(pd => pd.PostId == accountReport.PostId 
                //                                               && pd.PositionId == accountReport.PositionId));

                // Filter the PostPositions based on your condition
                //foreach (var detail in postRegistration.PostRegistrationDetails)
                //{
                //    detail.Position.Post.PostPositions = detail.Position.Post.PostPositions
                //                                    .Where(position => position.Id == accountReport.PositionId)
                //                                    .ToList();
                //}

                var postRegistration = await _unitOfWork.Repository<PostRegistration>().FindAsync(x => x.Id == accountReport.PositionId && x.AccountId == accountId);

                return new BaseResponseViewModel<ReportPostRegistrationResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<ReportPostRegistrationResponse>(postRegistration)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region private filter logic

        private static IQueryable<AccountReportResponse> FilterReportDate(IQueryable<AccountReportResponse> list, AccountReportFilter filter)
        {
            if (filter.CreateAtEnd != null && filter.CreateAtEnd.HasValue && filter.CreateAtStart != null && filter.CreateAtStart.HasValue)
            {
                //minus CreateAtEnd 1 day and set all time to mid night
                var startDate = filter.CreateAtStart.Value.Date;
                var endDate = filter.CreateAtEnd.Value.Date.AddDays(-1);

                //filter here
                list = list.Where(post => post.CreateAt >= startDate && post.CreateAt <= endDate);
            }

            //int size = list.Count();
            return list;
        }

        #endregion
    }
}
