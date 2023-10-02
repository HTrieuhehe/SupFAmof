using AutoMapper;
using AutoMapper.QueryableExtensions;
using NTQ.Sdk.Core.Utilities;
using Service.Commons;
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
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static SupFAmof.Service.Helpers.ErrorEnum;

namespace SupFAmof.Service.Service
{
    public class AccountBannedService : IAccountBannedService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AccountBannedService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponseViewModel<AccountBannedResponse>> CreateAccountBanned(int accountId, CreateAccountBannedRequest request)
        {
            try
            {
                var checkAdmission = _unitOfWork.Repository<Account>().GetAll().FirstOrDefault(x => x.Id == accountId);

                if (checkAdmission == null)
                {
                    throw new ErrorResponse(400, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                                    AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                //only admission have post permission can ban an account
                else if (checkAdmission.PostPermission == false)
                {
                    throw new ErrorResponse(403, (int)AccountBannedErrorEnum.ADMISSION_FORBIDDEN,
                                                    AccountBannedErrorEnum.ADMISSION_FORBIDDEN.GetDisplayName());
                }

                //check account has banned yet
                var accountBanned = _unitOfWork.Repository<AccountBanned>().GetAll().Where(x => x.Id == request.AccountIdBanned);

                //count how many banned time
                int bannedAttempt = accountBanned.Count();

                // get current Time for 
                var currentTime = Ultils.GetCurrentTime();

                //find if there is any banned day in range
                var checkBanned = accountBanned.FirstOrDefault(x => x.DayStart >= currentTime && x.DayStart <= request.DayEnd);

                if (checkBanned != null)
                {
                    throw new ErrorResponse(400, (int)AccountBannedErrorEnum.CREATE_BANNED_INVALID,
                                                   AccountBannedErrorEnum.CREATE_BANNED_INVALID.GetDisplayName()
                                                   + $": {checkBanned.DayEnd}");
                }

                if (bannedAttempt == 0)
                {
                    if (request.DayEnd < currentTime.AddDays(7))
                    {
                        throw new ErrorResponse(400, (int)AccountBannedErrorEnum.DAY_END_INVALID,
                                                       AccountBannedErrorEnum.DAY_END_INVALID.GetDisplayName()
                                                       + $": {currentTime.AddDays(7)} days because the account has been banned {bannedAttempt} times");
                    }

                    var accountBannedMappingAfter = _mapper.Map<AccountBanned>(request);
                    accountBannedMappingAfter.BannedPersonId = accountId;
                    accountBannedMappingAfter.DayStart = currentTime;
                    accountBannedMappingAfter.IsActive = true;

                    await _unitOfWork.Repository<AccountBanned>().InsertAsync(accountBannedMappingAfter);
                    await _unitOfWork.CommitAsync();

                    return new BaseResponseViewModel<AccountBannedResponse>()
                    {
                        Status = new StatusViewModel()
                        {
                            Message = "Success",
                            Success = true,
                            ErrorCode = 0
                        },
                        Data = _mapper.Map<AccountBannedResponse>(accountBannedMappingAfter)
                    };
                }    

                else if (request.DayEnd < currentTime.AddDays(7 * bannedAttempt))
                {
                    throw new ErrorResponse(400, (int)AccountBannedErrorEnum.DAY_END_INVALID,
                                                   AccountBannedErrorEnum.DAY_END_INVALID.GetDisplayName() 
                                                   + $": {currentTime.AddDays(7 * bannedAttempt)} days because the account has been banned {bannedAttempt} times");
                }

                var accountBannedMapping = _mapper.Map<AccountBanned>(request);
                accountBannedMapping.BannedPersonId = accountId;
                accountBannedMapping.DayStart = currentTime;
                accountBannedMapping.IsActive = true;

                await _unitOfWork.Repository<AccountBanned>().InsertAsync(accountBannedMapping);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AccountBannedResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountBannedResponse>(accountBannedMapping)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<AccountBannedResponse>> GetAccountBannedByToken(int accountId, PagingRequest paging)
        {
            try
            {
                var accountBanned = _unitOfWork.Repository<AccountBanned>().GetAll()
                                                .ProjectTo<AccountBannedResponse>(_mapper.ConfigurationProvider)
                                                .Where(x => x.AccountIdBanned == accountId)
                                                .PagingQueryable(paging.Page, paging.PageSize,
                                                           Constants.LimitPaging, Constants.DefaultPaging);
                                                

                if(!accountBanned.Item2.Any())
                {
                    throw new ErrorResponse(404, (int)AccountBannedErrorEnum.NOT_FOUND_BANNED_ACCOUNT,
                                                    AccountBannedErrorEnum.NOT_FOUND_BANNED_ACCOUNT.GetDisplayName());
                }

                return new BaseResponsePagingViewModel<AccountBannedResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = accountBanned.Item1
                    },
                    Data = accountBanned.Item2.ToList(),
                };
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<AccountBannedResponse>> GetAccountBanneds(AccountBannedResponse filter, PagingRequest paging)
        {
            try
            {
                var accountBanned = _unitOfWork.Repository<AccountBanned>().GetAll()
                                                .ProjectTo<AccountBannedResponse>(_mapper.ConfigurationProvider)
                                                .DynamicFilter(filter)
                                                .DynamicSort(filter)
                                                .PagingQueryable(paging.Page, paging.PageSize,
                                                           Constants.LimitPaging, Constants.DefaultPaging);

                return new BaseResponsePagingViewModel<AccountBannedResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = accountBanned.Item1
                    },
                    Data = accountBanned.Item2.ToList(),
                };
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AccountBannedResponse>> UpdateAccountBanned(int accountId, int accountBannedId, UpdateAccountBannedRequest request)
        {
            try
            {
                var accountBanned = _unitOfWork.Repository<AccountBanned>().Find(x => x.Id == accountBannedId && x.BannedPersonId == accountId);

                if (accountBanned == null)
                {
                    throw new ErrorResponse(400, (int)AccountBannedErrorEnum.NOT_FOUND_BANNED_ACCOUNT,
                                                    AccountBannedErrorEnum.NOT_FOUND_BANNED_ACCOUNT.GetDisplayName());
                }

                var updateBanned = _mapper.Map<UpdateAccountBannedRequest, AccountBanned>(request, accountBanned);

                await _unitOfWork.Repository<AccountBanned>().UpdateDetached(updateBanned);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AccountBannedResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountBannedResponse>(updateBanned)
                };
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
