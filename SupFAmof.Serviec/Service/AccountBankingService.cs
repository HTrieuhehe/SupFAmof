﻿using System;
using AutoMapper;
using System.Linq;
using System.Text;
using Service.Commons;
using SupFAmof.Data.Entity;
using System.Threading.Tasks;
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
using LAK.Sdk.Core.Utilities;
using Microsoft.EntityFrameworkCore;

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


        public async Task<BaseResponseViewModel<AccountBankingResponse>> GetAccountBankingByToken(int accountId)
        {
            try
            {
                var accountBanking = await _unitOfWork.Repository<AccountBanking>()
                                                      .GetAll()
                                                      .FirstOrDefaultAsync(x => x.AccountId == accountId && x.IsActive == true);
                if (accountBanking == null)
                {
                    throw new ErrorResponse(404, (int)AccountBankingErrorEnums.ACCOUNTBANKING_NOT_FOUND,
                                        AccountBankingErrorEnums.ACCOUNTBANKING_NOT_FOUND.GetDisplayName());
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

        public async Task<BaseResponsePagingViewModel<AccountBankingResponse>> GetAccountBankings(AccountBankingResponse filter, PagingRequest paging)
        {
            try
            {
                var accountBankings = _unitOfWork.Repository<AccountBanking>().GetAll()
                    .ProjectTo<AccountBankingResponse>(_mapper.ConfigurationProvider)
                    .DynamicFilter(filter)
                    .DynamicSort(paging.Sort, paging.Order)
                    .PagingQueryable(paging.Page, paging.PageSize);
                return new BaseResponsePagingViewModel<AccountBankingResponse>
                {
                    Metadata = new PagingsMetadata
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = accountBankings.Item1
                    },
                    Data = accountBankings.Item2.ToList(),
                };

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AccountBankingResponse>> CreateAccountBanking(int accountId, CreateAccountBankingRequest request)
        {
            try
            {
                //check banking information first

                var checkBankingInfo = await _unitOfWork.Repository<AccountBanking>()
                                                        .GetAll()
                                                        .FirstOrDefaultAsync(x => x.AccountId == accountId && x.IsActive == true);

                #region checking validation

                if (checkBankingInfo != null)
                {
                    throw new ErrorResponse(400, (int)AccountBankingErrorEnums.ACCOUNT_BANKING_EXISTED,
                                        AccountBankingErrorEnums.ACCOUNT_BANKING_EXISTED.GetDisplayName());
                }

                else if (string.IsNullOrEmpty(request.AccountNumber))
                {

                    throw new ErrorResponse(400, (int)AccountBankingErrorEnums.ACCOUNT_BANKING_NUMBER_NOT_NULL,
                                    AccountBankingErrorEnums.ACCOUNT_BANKING_NUMBER_NOT_NULL.GetDisplayName());
                }

                //check Account Number is number or contain char
                else if (!Ultils.CheckIsOnlyNumber(request.AccountNumber))
                {
                    throw new ErrorResponse(400, (int)AccountBankingErrorEnums.ACCOUNT_BAKING_NUMBER_INVALID,
                                        AccountBankingErrorEnums.ACCOUNT_BAKING_NUMBER_INVALID.GetDisplayName());
                }

                #endregion

                var account = _mapper.Map<AccountBanking>(request);

                account.AccountId = accountId;

                //loại bỏ các dấu thanh âm => Ví dụ (HỒ hố hô -> Ho ho ho)
                account.Beneficiary = Ultils.RemoveDiacritics(account.Beneficiary).ToUpper();
                account.Branch = Ultils.RemoveDiacritics(account.Branch).ToUpper();
                account.IsActive = true;
                account.CreateAt = Ultils.GetCurrentDatetime();

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

        public async Task<BaseResponseViewModel<AccountBankingResponse>> UpdateAccountBanking(int accountId, UpdateAccountBankingRequest request)
        {
            try
            {
                //check banking existed or not
                var checkBankingInfo = await _unitOfWork.Repository<AccountBanking>()
                                                       .GetAll()
                                                       .FirstOrDefaultAsync(x => x.AccountId == accountId && x.IsActive == true);

                #region checking validation

                if (checkBankingInfo == null)
                {
                    throw new ErrorResponse(404, (int)AccountBankingErrorEnums.ACCOUNTBANKING_NOT_FOUND,
                                        AccountBankingErrorEnums.ACCOUNTBANKING_NOT_FOUND.GetDisplayName());
                }

                else if (request.AccountNumber == null || request.AccountNumber == "")
                {

                    throw new ErrorResponse(400, (int)AccountBankingErrorEnums.ACCOUNT_BANKING_NUMBER_NOT_NULL,
                                    AccountBankingErrorEnums.ACCOUNT_BANKING_NUMBER_NOT_NULL.GetDisplayName());
                }

                //check Account Number is number or contain char
                else if (!Ultils.CheckIsOnlyNumber(request.AccountNumber))
                {
                    throw new ErrorResponse(400, (int)AccountBankingErrorEnums.ACCOUNT_BAKING_NUMBER_INVALID,
                                        AccountBankingErrorEnums.ACCOUNT_BAKING_NUMBER_INVALID.GetDisplayName());
                }

                #endregion

                //create if accountBanking not have any data
                //if (bankingCheck == null)
                //{
                //    request.Beneficiary = Ultils.RemoveDiacritics(request.Beneficiary).ToUpper();
                //    request.Branch = Ultils.RemoveDiacritics(request.Branch).ToUpper();

                //    CreateAccountBankingRequest createAccountBanking = new()
                //    {
                //        Beneficiary = request.Beneficiary,
                //        Branch = request.Branch,
                //        AccountNumber = request.AccountNumber,
                //        BankName = request.BankName,
                //    };

                //    var bankingMapping = _mapper.Map<AccountBanking>(createAccountBanking);
                //    bankingMapping.IsActive = true;
                //    bankingMapping.CreateAt = Ultils.GetCurrentDatetime();

                //    await _unitOfWork.Repository<AccountBanking>().InsertAsync(bankingMapping);
                //    await _unitOfWork.CommitAsync();

                //    return new BaseResponseViewModel<AccountBankingResponse>()
                //    {
                //        Status = new StatusViewModel()
                //        {
                //            Message = "Success",
                //            Success = true,
                //            ErrorCode = 0
                //        },
                //        Data = _mapper.Map<AccountBankingResponse>(account)
                //    };
                //}

                var accountBanking = _mapper.Map<UpdateAccountBankingRequest, AccountBanking>(request, checkBankingInfo);

                //loại bỏ các dấu thanh âm => Ví dụ (HỒ hố hô -> Ho ho ho)
                accountBanking.Beneficiary = Ultils.RemoveDiacritics(accountBanking.Beneficiary).ToUpper();
                accountBanking.Branch = Ultils.RemoveDiacritics(accountBanking.Branch).ToUpper();
                accountBanking.UpdateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<AccountBanking>().UpdateDetached(accountBanking);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AccountBankingResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountBankingResponse>(accountBanking)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
