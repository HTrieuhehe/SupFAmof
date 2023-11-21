using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml.Office2016.Excel;
using LAK.Sdk.Core.Utilities;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Service.Commons;
using ServiceStack.Web;
using SupFAmof.Data.Entity;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Admission;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Service.ServiceInterface;
using SupFAmof.Service.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SupFAmof.Service.Helpers.Enum;
using static SupFAmof.Service.Helpers.ErrorEnum;

namespace SupFAmof.Service.Service
{
    public class AccountCertificateService : IAccountCertificateService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AccountCertificateService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponseViewModel<AccountCertificateResponse>> CreateAccountCertificate(int certificateIssuerId, CreateAccountCertificateRequest request)
        {
            // Mỗi collab account có 1 và chỉ 1 certi/loại. Có thể có certi A, B, C
            // nhưng không được có > 1 certi cùng loại 

            try
            {
                //check account post Permission
                var checkAccount = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == certificateIssuerId);

                if (checkAccount == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                else if (checkAccount.PostPermission == false)
                {
                    throw new ErrorResponse(403, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                //check accountId is collaborator or not
                var checkCollaboratorAccount = await _unitOfWork.Repository<Account>()
                                                    .FindAsync(x => x.Id == request.AccountId && x.RoleId == (int)SystemRoleEnum.Collaborator);

                if (checkCollaboratorAccount == null)
                {
                    throw new ErrorResponse(400, (int)AccountCertificateErrorEnum.ACCOUNT_COLLABORATOR_INVALID,
                                         AccountCertificateErrorEnum.ACCOUNT_COLLABORATOR_INVALID.GetDisplayName());
                }

                //check certificate
                var checkCertificate = await _unitOfWork.Repository<TrainingCertificate>()
                                                    .FindAsync(x => x.Id == request.TrainingCertificateId);

                if (checkCertificate == null)
                {
                    throw new ErrorResponse(404, (int)TrainingCertificateErrorEnum.NOT_FOUND_ID,
                                         TrainingCertificateErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                // check whether account certificate already had or not
                var checkAccountCertificate = await _unitOfWork.Repository<AccountCertificate>().GetAll()
                                                  .FirstOrDefaultAsync(a => a.AccountId == request.AccountId 
                                                                        && a.TrainingCertificateId == request.TrainingCertificateId);
                
                if (checkAccountCertificate != null) 
                {
                    throw new ErrorResponse(400, (int)AccountCertificateErrorEnum.ACCOUNT_CERTIFICATE_EXISTED,
                                         AccountCertificateErrorEnum.ACCOUNT_CERTIFICATE_EXISTED.GetDisplayName());
                }

                var result = _mapper.Map<CreateAccountCertificateRequest, AccountCertificate>(request);

                result.CertificateIssuerId = certificateIssuerId;
                result.Status = (int)AccountCertificateStatusEnum.Complete;
                result.CreateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<AccountCertificate>().InsertAsync(result);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AccountCertificateResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountCertificateResponse>(result)
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AccountCertificateResponse>> GetAccountCertificateById(int accountCertiId)
        {
            try
            {
                var postTitle = _unitOfWork.Repository<AccountCertificate>().GetAll()
                                      .FirstOrDefault(x => x.Id == accountCertiId);

                if (postTitle == null)
                {
                    throw new ErrorResponse(404, (int)AccountCertificateErrorEnum.NOT_FOUND_ID,
                                         AccountCertificateErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                return new BaseResponseViewModel<AccountCertificateResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountCertificateResponse>(postTitle)
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }

        public async Task<BaseResponsePagingViewModel<AccountCertificateResponse>> GetAccountCertificates(AccountCertificateResponse filter, PagingRequest paging)
        {
            try
            {
                var accountCerti = _unitOfWork.Repository<AccountCertificate>().GetAll()
                                    .ProjectTo<AccountCertificateResponse>(_mapper.ConfigurationProvider)
                                    .OrderByDescending(x => x.CreateAt)
                                    .DynamicFilter(filter)
                                    .DynamicSort(paging.Sort, paging.Order)
                                    .PagingQueryable(paging.Page, paging.PageSize,
                                    Constants.LimitPaging, Constants.DefaultPaging);

                return new BaseResponsePagingViewModel<AccountCertificateResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = accountCerti.Item1
                    },
                    Data = accountCerti.Item2.ToList()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AccountCertificateResponse>> UpdateAccountCertificate(int certificateIssuerId, UpdateAccountCertificateRequest request)
        {
            // chỉ có người tạo certi cho account mới có quyền update status
            // Mỗi collab account có 1 và chỉ 1 certi/loại. Có thể có certi A, B, C
            // nhưng không được có > 1 certi cùng loại 
            try
            {
                //check account post Permission
                var checkAccount = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == certificateIssuerId);

                if (checkAccount == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                else if (checkAccount.PostPermission == false)
                {
                    throw new ErrorResponse(403, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                //check accountId is collaborator or not
                var checkCollaboratorAccount = await _unitOfWork.Repository<Account>()
                                                    .FindAsync(x => x.Id == request.AccountId && x.RoleId == (int)SystemRoleEnum.Collaborator);

                if (checkCollaboratorAccount == null)
                {
                    throw new ErrorResponse(400, (int)AccountCertificateErrorEnum.ACCOUNT_COLLABORATOR_INVALID,
                                         AccountCertificateErrorEnum.ACCOUNT_COLLABORATOR_INVALID.GetDisplayName());
                }

                var accountCertificate = await _unitOfWork.Repository<AccountCertificate>()
                                                  .FindAsync(a => a.AccountId == request.AccountId
                                                  && a.Id == request.TrainingCertificateId 
                                                  && a.CertificateIssuerId == certificateIssuerId);

                if (accountCertificate == null)
                {
                    throw new ErrorResponse(400, (int)AccountCertificateErrorEnum.NOT_FOUND_ID,
                                         AccountCertificateErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                if (accountCertificate.Status == request.Status)
                {
                    throw new ErrorResponse(400, (int)AccountCertificateErrorEnum.STATUS_ALREADY_SAME,
                                         AccountCertificateErrorEnum.STATUS_ALREADY_SAME.GetDisplayName());
                }

                accountCertificate.UpdateAt = Ultils.GetCurrentDatetime();

                return new BaseResponseViewModel<AccountCertificateResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountCertificateResponse>(accountCertificate)
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<AccountCertificateResponse>> GetAccountCertificateByAccountId(int accountId, AccountCertificateResponse filter, PagingRequest paging)
        {
            try
            {

                var accountCerti = _unitOfWork.Repository<AccountCertificate>().GetAll()
                                    .Where(a => a.AccountId == accountId)
                                    .ProjectTo<AccountCertificateResponse>(_mapper.ConfigurationProvider)
                                    .DynamicFilter(filter)
                                    .DynamicSort(paging.Sort, paging.Order)
                                    .OrderByDescending(x => x.CreateAt)
                                    .PagingQueryable(paging.Page, paging.PageSize);

                return new BaseResponsePagingViewModel<AccountCertificateResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = accountCerti.Item1
                    },
                    Data = accountCerti.Item2.ToList()
                };
            }
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }
    }
}
