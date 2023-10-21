using System;
using AutoMapper;
using System.Linq;
using System.Text;
using Service.Commons;
using SupFAmof.Data.Entity;
using System.Threading.Tasks;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.Utilities;
using System.Collections.Generic;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using AutoMapper.QueryableExtensions;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.ErrorEnum;
using LAK.Sdk.Core.Utilities;

namespace SupFAmof.Service.Service
{
    public class ComplaintService : IComplaintService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ComplaintService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponseViewModel<CompaintResponse>> CreateAccountReportProblem(int accountId, CreateAccountReportProblemRequest request)
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

                var report = _mapper.Map<CreateAccountReportProblemRequest, Complaint>(request);

                report.AccountId = accountId;
                report.ReportDate = Ultils.GetCurrentDatetime();
                report.Status = (int)ReportProblemStatusEnum.Pending;

                await _unitOfWork.Repository<Complaint>().InsertAsync(report);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<CompaintResponse>
                {
                    Status = new StatusViewModel
                    {
                        Message = "Success",
                        ErrorCode = 0,
                        Success = true,
                    },
                    Data = _mapper.Map<CompaintResponse>(report)

                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<CompaintResponse>> GetAccountReportProblemsByToken(int accountId, CompaintResponse filter, PagingRequest paging)
        {
            try
            {
                var reportProblem = _unitOfWork.Repository<Complaint>().GetAll()
                                               .Where(x => x.AccountId == accountId)
                                               .OrderByDescending(x => x.ReportDate)
                                               .ProjectTo<CompaintResponse>(_mapper.ConfigurationProvider)
                                               .DynamicFilter(filter)
                                               .DynamicSort(paging.Sort, paging.Order)
                                                .PagingQueryable(paging.Page, paging.PageSize);

                return new BaseResponsePagingViewModel<CompaintResponse>()
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

        public async Task<BaseResponsePagingViewModel<AdmissionComplaintResponse>> GetAdmissionAccountReportProblems(int accountId, AdmissionComplaintResponse filter, PagingRequest paging)
        {
            try
            {
                //check admission permission

                var checkAdmission = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);

                if (checkAdmission == null )
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                var reportProblem = _unitOfWork.Repository<Complaint>().GetAll()
                                               .ProjectTo<AdmissionComplaintResponse>(_mapper.ConfigurationProvider)
                                               .OrderByDescending(x => x.ReportDate)
                                               .DynamicFilter(filter)
                                               .DynamicSort(paging.Sort, paging.Order)
                                               .PagingQueryable(paging.Page, paging.PageSize);

                return new BaseResponsePagingViewModel<AdmissionComplaintResponse>()
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

        public async Task<BaseResponseViewModel<AdmissionComplaintResponse>> RejectReportProblem(int accountId, int reportId, UpdateAdmissionAccountReportProblemRequest request)
        {
            try
            {
                var checkAdmission = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);

                if (checkAdmission == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                var report = await _unitOfWork.Repository<Complaint>().FindAsync(r => r.Id == reportId);

                if (report == null)
                {
                    throw new ErrorResponse(404, (int)ReportProblemErrorEnum.NOT_FOUND_REPORT,
                                        ReportProblemErrorEnum.NOT_FOUND_REPORT.GetDisplayName());
                }
                switch (report.Status)
                {
                    case (int)ReportProblemStatusEnum.Approve:
                        throw new ErrorResponse(400,
                            (int)ReportProblemErrorEnum.ALREADY_APPROVE,
                            ReportProblemErrorEnum.ALREADY_APPROVE.GetDisplayName());

                    // Add more cases here if needed
                    case (int)ReportProblemStatusEnum.Reject:
                        throw new ErrorResponse(400,
                            (int)ReportProblemErrorEnum.ALREADY_REJECT,
                            ReportProblemErrorEnum.ALREADY_REJECT.GetDisplayName());

                    default:
                        break;
                }

                var replyReport = _mapper.Map<UpdateAdmissionAccountReportProblemRequest, Complaint>(request, report);

                replyReport.ReplyDate = Ultils.GetCurrentDatetime();
                replyReport.Status = (int)ReportProblemStatusEnum.Reject;

                await _unitOfWork.Repository<Complaint>().UpdateDetached(replyReport);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AdmissionComplaintResponse>
                {
                    Status = new StatusViewModel
                    {
                        Message = "Success",
                        ErrorCode = 0,
                        Success = true,
                    },
                    Data = _mapper.Map<AdmissionComplaintResponse>(report)

                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AdmissionComplaintResponse>> ApproveReportProblem(int accountId, int reportId, UpdateAdmissionAccountReportProblemRequest request)
        {
            try
            {
                var checkAdmission = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);

                if (checkAdmission == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                var report = await _unitOfWork.Repository<Complaint>().FindAsync(r => r.Id == reportId);

                if (report == null)
                {
                    throw new ErrorResponse(404, (int)ReportProblemErrorEnum.NOT_FOUND_REPORT,
                                        ReportProblemErrorEnum.NOT_FOUND_REPORT.GetDisplayName());
                }
                switch (report.Status)
                {
                    case (int)ReportProblemStatusEnum.Approve:
                        throw new ErrorResponse(400,
                            (int)ReportProblemErrorEnum.ALREADY_APPROVE,
                            ReportProblemErrorEnum.ALREADY_APPROVE.GetDisplayName());

                    // Add more cases here if needed
                    case (int)ReportProblemStatusEnum.Reject:
                        throw new ErrorResponse(400,
                            (int)ReportProblemErrorEnum.ALREADY_REJECT,
                            ReportProblemErrorEnum.ALREADY_REJECT.GetDisplayName());

                    default:
                        break;
                }

                var replyReport = _mapper.Map<UpdateAdmissionAccountReportProblemRequest, Complaint>(request, report);

                replyReport.ReplyDate = Ultils.GetCurrentDatetime();
                replyReport.Status = (int)ReportProblemStatusEnum.Approve;

                await _unitOfWork.Repository<Complaint>().UpdateDetached(replyReport);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AdmissionComplaintResponse>
                {
                    Status = new StatusViewModel
                    {
                        Message = "Success",
                        ErrorCode = 0,
                        Success = true,
                    },
                    Data = _mapper.Map<AdmissionComplaintResponse>(report)

                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
