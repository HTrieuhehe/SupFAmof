﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using FirebaseAdmin.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NTQ.Sdk.Core.Utilities;
using Service.Commons;
using ServiceStack.Web;
using SupFAmof.Data.Entity;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Account;
using SupFAmof.Service.DTO.Request.Admission.AccountRequest;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Helpers;
using SupFAmof.Service.Service.ServiceInterface;
using SupFAmof.Service.Service.ServiceInterface.AdmissionIService;
using SupFAmof.Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SupFAmof.Service.Helpers.ErrorEnum;

namespace SupFAmof.Service.Service.AdmissionService
{
    public class AdmissionAccountService : IAdmissionAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IFcmTokenService _accountFcmtokenService;

        public AdmissionAccountService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, IFcmTokenService accountFcmtokenService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _accountFcmtokenService = accountFcmtokenService;
        }

        public async Task<BaseResponseViewModel<AdmissionAccountResponse>> CreateAccount(CreateAccountRequest request)
        {
            try
            {
                var account = _mapper.Map<CreateAccountRequest, Account>(request);

                #region Identify role by email root
                string[] splitEmail = account.Email.Split('@');
                var rootEmail = splitEmail[1];

                //sau khi tách, lấy giá trị [1] tức là đuôi domain đằng sau dấu @
                var roleInfo = _unitOfWork.Repository<Role>().GetAll()
                                          .FirstOrDefault(x => x.RoleEmail.Contains(rootEmail));
                if (roleInfo == null)
                {
                    throw new ErrorResponse(400, (int)RoleErrorEnums.ROLE_NOTE_FOUND,
                                        RoleErrorEnums.ROLE_NOTE_FOUND.GetDisplayName());
                }
                #endregion

                account.RoleId = roleInfo.Id;
                account.AccountInformationId = 0;
                account.PostPermission = false;
                account.IsPremium = false;
                account.IsActive = true;
                account.CreateAt = DateTime.Now;

                await _unitOfWork.Repository<Account>().InsertAsync(account);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AdmissionAccountResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdmissionAccountResponse>(account)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AdmissionAccountResponse>> DisableAccount(int accountId)
        {
            Account account = _unitOfWork.Repository<Account>()
                                         .Find(x => x.Id == accountId);

            if (account == null)
            {
                throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                    AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
            }

            account.UpdateAt = DateTime.Now;
            account.IsActive = false;

            await _unitOfWork.Repository<Account>().UpdateDetached(account);
            await _unitOfWork.CommitAsync();

            return new BaseResponseViewModel<AdmissionAccountResponse>()
            {
                Status = new StatusViewModel()
                {
                    Message = "Success",
                    Success = true,
                    ErrorCode = 0
                },
                Data = _mapper.Map<AdmissionAccountResponse>(account)
            };
        }

        public async Task<BaseResponseViewModel<AdmissionAccountResponse>> GetAccountByEmail(string email)
        {
            try
            {
                var account = _unitOfWork.Repository<Account>().GetAll()
                                                .Where(x => x.Email.Contains(email)).FirstOrDefault();

                return new BaseResponseViewModel<AdmissionAccountResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdmissionAccountResponse>(account)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AdmissionAccountResponse>> GetAccountById(int accountId)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().GetAll()
                                         .Where(x => x.Id == accountId)
                                         .FirstOrDefaultAsync();

                if (account == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                return new BaseResponseViewModel<AdmissionAccountResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdmissionAccountResponse>(account)
                };
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<AdmissionAccountResponse>> GetAccounts(AdmissionAccountResponse filter, PagingRequest paging)
        {
            try
            {
                var account = _unitOfWork.Repository<Account>().GetAll()
                    .ProjectTo<AdmissionAccountResponse>(_mapper.ConfigurationProvider)
                    .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);

                return new BaseResponsePagingViewModel<AdmissionAccountResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = account.Item1
                    },
                    Data = account.Item2.ToList()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<LoginResponse>> Login(ExternalAuthRequest data)
        {
            try
            {
                //check fcm token
                if (data.FcmToken != null && data.FcmToken.Trim().Length > 0)
                {
                    if (!await _accountFcmtokenService.ValidToken(data.FcmToken))
                        throw new ErrorResponse(400, (int)FcmTokenErrorEnums.INVALID_TOKEN,
                                             FcmTokenErrorEnums.INVALID_TOKEN.GetDisplayName());
                }

                #region Decode Token

                //tham chiếu đến phiên bản mặc định của lớp FirebaseAuth từ Firebase Admin SDK. FirebaseAuth cung cấp
                //các phương thức và chức năng để quản lý và xác thực người dùng trong Firebase
                var auth = FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance;

                //Phương thức này sẽ xác thực và giải mã một token được cung cấp từ người dùng. Nó sẽ xác minh tính hợp lệ của token, kiểm tra chữ ký và trích xuất thông tin từ token.
                //Kết quả trả về là một đối tượng FirebaseToken chứa thông tin được giải mã từ token
                FirebaseToken decodeToken = await auth.VerifyIdTokenAsync(data.IdToken);

                //Phương thức này lấy thông tin người dùng từ Firebase dựa trên Uid (User ID) được trích xuất từ token đã giải mã trước đó.
                UserRecord userRecord = await auth.GetUserAsync(decodeToken.Uid);

                #endregion

                //check exist account
                var account = _unitOfWork.Repository<Account>().GetAll()
                                         .FirstOrDefault(x => x.Email.Contains(userRecord.Email));

                //create new Account
                if (account == null)
                {
                    CreateAccountRequest newAccount = new CreateAccountRequest()
                    {
                        Name = userRecord.DisplayName,
                        Email = userRecord.Email,
                        Phone = userRecord.PhoneNumber,
                        ImgUrl = userRecord.PhotoUrl
                    };

                    //create account
                    await CreateAccount(newAccount);

                    //Get Account Created right now
                    account = _unitOfWork.Repository<Account>().GetAll()
                                .FirstOrDefault(x => x.Email.Contains(userRecord.Email));

                    //generate token
                    var newToken = AccessTokenManager.GenerateJwtToken(string.IsNullOrEmpty(account.Name) ? "" : account.Name, account.RoleId, account.Id, _configuration);

                    //Add fcm token 
                    if (data.FcmToken != null && data.FcmToken.Trim().Length > 0)
                        _accountFcmtokenService.AddFcmToken(data.FcmToken, account.Id);

                    return new BaseResponseViewModel<LoginResponse>()
                    {
                        Status = new StatusViewModel()
                        {
                            Message = "Success",
                            Success = true,
                            ErrorCode = 0
                        },
                        Data = new LoginResponse()
                        {
                            access_token = newToken,
                            account = _mapper.Map<AccountResponse>(account)
                        }
                    };
                }

                else
                {
                    //generate token
                    var newToken = AccessTokenManager.GenerateJwtToken(string.IsNullOrEmpty(account.Name) ? "" : account.Name, 0, account.Id, _configuration);

                    //Add fcm token 
                    if (data.FcmToken != null && data.FcmToken.Trim().Length > 0)
                        _accountFcmtokenService.AddFcmToken(data.FcmToken, account.Id);

                    return new BaseResponseViewModel<LoginResponse>()
                    {
                        Status = new StatusViewModel()
                        {
                            Message = "Success",
                            Success = true,
                            ErrorCode = 0
                        },
                        Data = new LoginResponse()
                        {
                            access_token = newToken,
                            account = _mapper.Map<AccountResponse>(account)
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task Logout(string fcmToken)
        {
            if (fcmToken != null && !fcmToken.Trim().Equals("") && !await _accountFcmtokenService.ValidToken(fcmToken))
            {
                _accountFcmtokenService.RemoveFcmTokens(new List<string> { fcmToken });
            }
        }

        public async Task<BaseResponseViewModel<AdmissionAccountResponse>> UpdateAccount(int accountId, UpdateAdmissionAccountRequest request)
        {
            try
            {
                Account account = _unitOfWork.Repository<Account>()
                                         .Find(x => x.Id == accountId);

                if (account == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }


                var checkPhone = Ultils.CheckVNPhone(request.Phone);

                if (checkPhone == false)
                {
                    throw new ErrorResponse(400, (int)AccountErrorEnums.ACCOUNT_PHONE_INVALID,
                                        AccountErrorEnums.ACCOUNT_PHONE_INVALID.GetDisplayName());
                }

                account = _mapper.Map<UpdateAdmissionAccountRequest, Account>(request, account);

                account.UpdateAt = DateTime.Now;

                await _unitOfWork.Repository<Account>().UpdateDetached(account);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AdmissionAccountResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdmissionAccountResponse>(account)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
