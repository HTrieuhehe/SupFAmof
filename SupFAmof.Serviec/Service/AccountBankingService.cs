using System;
using AutoMapper;
using System.Linq;
using System.Text;
using Service.Commons;
using SupFAmof.Data.Entity;
using System.Threading.Tasks;
using NTQ.Sdk.Core.Utilities;
using SupFAmof.Data.UnitOfWork;
using System.Security.Principal;
using System.Collections.Generic;
using SupFAmof.Service.Utilities;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Configuration;
using SupFAmof.Service.DTO.Request.Account;
using static SupFAmof.Service.Helpers.ErrorEnum;
using SupFAmof.Service.DTO.Request.AccounBanking;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof.Service.Service
{
    public class AccountBankingService : IAccountBankingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountBankingService
            (IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<BaseResponseViewModel<AccountBankingResponse>> GetAccountBankingById(int id)
        {
            try
            {
                var accountBanking = await _unitOfWork.Repository<AccountBanking>().GetById(id);
                if (accountBanking == null)
                {
                    throw new ErrorResponse(404, (int)AccountBankingErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountBankingErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }
                return new BaseResponseViewModel<AccountBankingResponse>
                {
                    Status = new StatusViewModel
                    {
                        Message = "Success",
                        ErrorCode = 0,
                        Success = true,
                    },
                    Data = _mapper.Map<AccountBankingResponse>(accountBanking)
                   
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<BaseResponsePagingViewModel<AccountBankingResponse>> GetAccountBankings(AccountBankingResponse request,PagingRequest paging)
        {
            try
            {
                var accountBankings=  _unitOfWork.Repository<AccountBanking>().GetAll()
                    .ProjectTo<AccountBankingResponse>(_mapper.ConfigurationProvider)
                    .DynamicFilter(request)
                    .DynamicSort(request)
                    .PagingQueryable(paging.Page,paging.PageSize,Constants.LimitPaging,Constants.DefaultPaging);
                return new BaseResponsePagingViewModel<AccountBankingResponse>
                {
                    Metadata = new PagingsMetadata
                    {Page=paging.Page,
                    Size=paging.PageSize,
                    Total=accountBankings.Item1
                    },
                    Data = accountBankings.Item2.ToList(),
                };

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<AccountBankingResponse>> CreateAccountBanking(CreateAccountBankingRequest request)
        {
            try
            {
                var account = _mapper.Map<AccountBanking>(request);

                account.Beneficiary = account.Beneficiary.ToUpper();
                account.IsActive= true;
                account.CreateAt= DateTime.Now;
                await _unitOfWork.Repository<AccountBanking>().InsertAsync(account);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AccountBankingResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountBankingResponse>(account)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<BaseResponseViewModel<AccountBankingResponse>> UpdateAccountBanking(int accountBankingId, UpdateAccountBankingRequest request)
        {
            try
            {
                AccountBanking account = _unitOfWork.Repository<AccountBanking>()
                                         .Find(x => x.Id == accountBankingId);

                if (account == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                account = _mapper.Map<UpdateAccountBankingRequest, AccountBanking>(request, account);
                account.Beneficiary = account.Beneficiary.ToUpper();
                account.UpdateAt = DateTime.Now;
                await _unitOfWork.Repository<AccountBanking>().UpdateDetached(account);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AccountBankingResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountBankingResponse>(account)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
