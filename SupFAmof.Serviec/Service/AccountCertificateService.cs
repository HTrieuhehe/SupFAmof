using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NTQ.Sdk.Core.Utilities;
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SupFAmof.Service.Helpers.ErrorEnum;

namespace SupFAmof.Service.Service
{
    public class AccountCertificateService : IAccountCertificateService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AccountCertificateService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponseViewModel<AccountCertificateResponse>> CreateAccountCertificate(int createPersonId, CreateAccountCertificateRequest request)
        {
            // Mỗi collab account có 1 và chỉ 1 certi/loại. Có thể có certi A, B, C
            // nhưng không được có > 1 certi cùng loại 

            try
            {
                var checkCertificate = _unitOfWork.Repository<AccountCertificate>().GetAll()
                                                  .FirstOrDefault(a => a.AccountId == request.AccountId && a.TraningCertificateId == request.TraningCertificateId);
                
                if (checkCertificate != null) 
                {
                    throw new ErrorResponse(400, (int)AccountCertificateErrorEnum.ACCOUNT_CERTIFICATE_EXISTED,
                                         AccountCertificateErrorEnum.ACCOUNT_CERTIFICATE_EXISTED.GetDisplayName());
                }

                var result = _mapper.Map<CreateAccountCertificateRequest, AccountCertificate>(request);

                result.CreatePersonId = createPersonId;
                result.Status = true;
                result.CreateAt = DateTime.Now;

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
                                    .DynamicFilter(filter)
                                    .DynamicSort(filter)
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

        public async Task<BaseResponseViewModel<AccountCertificateResponse>> UpdateAccountCertificate(int accountId, int traningCertificateId, int createPersonId)
        {
            // chỉ có người tạo certi cho account mới có quyền update status
            // Mỗi collab account có 1 và chỉ 1 certi/loại. Có thể có certi A, B, C
            // nhưng không được có > 1 certi cùng loại 
            try
            {
                var accountCertificate = _unitOfWork.Repository<AccountCertificate>().GetAll()
                                                  .FirstOrDefault(a => a.AccountId == accountId && a.TraningCertificateId == traningCertificateId && a.CreatePersonId == createPersonId);

                if (accountCertificate == null)
                {
                    throw new ErrorResponse(400, (int)AccountCertificateErrorEnum.NOT_FOUND_ID,
                                         AccountCertificateErrorEnum.NOT_FOUND_ID.GetDisplayName());
                }

                accountCertificate.Status = false;
                accountCertificate.UpdateAt = DateTime.Now;

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

        public async Task<BaseResponsePagingViewModel<AccountCertificateResponse>> GetAccountCertificateByAccountId(int accountId, PagingRequest paging)
        {
            try
            {

                var accountCerti = _unitOfWork.Repository<AccountCertificate>().GetAll()
                                    .Where(a => a.AccountId == accountId)
                                    .ProjectTo<AccountCertificateResponse>(_mapper.ConfigurationProvider)
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
            catch (ErrorResponse ex)
            {
                throw ex;
            }
        }
    }
}
