using AutoMapper;
using System.Data;
using Service.Commons;
using FirebaseAdmin.Auth;
using SupFAmof.Data.Entity;
using LAK.Sdk.Core.Utilities;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.Helpers;
using SupFAmof.Service.Utilities;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using Microsoft.EntityFrameworkCore;
using SupFAmof.Service.DTO.Response;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Configuration;
using SupFAmof.Service.DTO.Request.Account;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.ErrorEnum;
using SupFAmof.Service.DTO.Request.Admission.AccountRequest;
using DocumentFormat.OpenXml.VariantTypes;

namespace SupFAmof.Service.Service
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IExpoTokenService _accountExpoTokenService;
        private readonly ISendMailService _sendMailService;

        public AccountService
            (IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, IExpoTokenService accountExpotokenService, ISendMailService sendMailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _accountExpoTokenService = accountExpotokenService;
            _sendMailService = sendMailService;
        }

        public async Task<BaseResponseViewModel<LoginResponse>> AdmissionLogin(ExternalAuthRequest data)
        {
            try
            {
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

                #region Identify role by email root to validate

                var identifyRootEmail = await ValidateRootEmail(userRecord.Email, (int)SystemRoleEnum.AdmissionManager);

                if (!identifyRootEmail)
                {
                    throw new ErrorResponse(403, (int)AccountErrorEnums.API_INVALID,
                                                AccountErrorEnums.API_INVALID.GetDisplayName());
                }

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

                    //Add expo token 
                    if (data.ExpoPushToken != null && data.ExpoPushToken.Trim().Length > 0)
                        _accountExpoTokenService.AddExpoToken(data.ExpoPushToken, account.Id);

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

                else if (account.IsActive == false)
                {
                    //generate token
                    var newToken = AccessTokenManager.GenerateJwtToken(string.IsNullOrEmpty(account.Name) ? "" : account.Name, account.RoleId, account.Id, _configuration);

                    //Add fcm token     
                    if (data.ExpoPushToken != null && data.ExpoPushToken.Trim().Length > 0)
                        _accountExpoTokenService.AddExpoToken(data.ExpoPushToken, account.Id);

                    return new BaseResponseViewModel<LoginResponse>()
                    {
                        Status = new StatusViewModel()
                        {
                            Message = "Account is not active",
                            Success = false,
                            ErrorCode = 4009
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
                    var newToken = AccessTokenManager.GenerateJwtToken(string.IsNullOrEmpty(account.Name) ? "" : account.Name, account.RoleId, account.Id, _configuration);

                    //Add fcm token     
                    if (data.ExpoPushToken != null && data.ExpoPushToken.Trim().Length > 0)
                        _accountExpoTokenService.AddExpoToken(data.ExpoPushToken, account.Id);

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

        public async Task<BaseResponseViewModel<AccountResponse>> CreateAccount(CreateAccountRequest request)
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
                account.AccountInformationId = null;
                account.PostPermission = false;
                account.IsPremium = false;
                account.IsActive = true;
                account.CreateAt = DateTime.Now;

                await _unitOfWork.Repository<Account>().InsertAsync(account);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AccountResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountResponse>(account)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AccountResponse>> DisableAccount(int accountId)
        {
            Account account = _unitOfWork.Repository<Account>()
                                         .Find(x => x.Id == accountId);

            if (account.IsActive == false)
            {
                throw new ErrorResponse(400, (int)AccountErrorEnums.ACCOUNT_DISABLE,
                                    AccountErrorEnums.ACCOUNT_DISABLE.GetDisplayName());
            }

            else if (account == null)
            {
                throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                    AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
            }

            var expoToken = await _unitOfWork.Repository<ExpoPushToken>().GetAll().FirstOrDefaultAsync(x => x.AccountId == accountId);
            var reactivationAccount = _unitOfWork.Repository<AccountReactivation>().Find(x => x.AccountId == accountId);

            if (reactivationAccount == null)
            {
                account.IsActive = false;

                //create new data
                CreateAccountReactivationRequest accountReactivation = new CreateAccountReactivationRequest()
                {
                    AccountId = account.Id,
                    Email = account.Email,
                };

                var accountReactivationMapping = _mapper.Map<CreateAccountReactivationRequest, AccountReactivation>(accountReactivation);
                accountReactivationMapping.DeactivateDate = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<Account>().UpdateDetached(account);
                await _unitOfWork.Repository<AccountReactivation>().InsertAsync(accountReactivationMapping);
                await _unitOfWork.CommitAsync();

                if (expoToken != null)
                {
                    ExpoTokenLogoutRequest token = new()
                    {
                        ExpoPushToken = expoToken.Token
                    };

                    await Logout(token, accountId, 2);

                    return new BaseResponseViewModel<AccountResponse>()
                    {
                        Status = new StatusViewModel()
                        {
                            Message = "Success",
                            Success = true,
                            ErrorCode = 0
                        },
                        Data = _mapper.Map<AccountResponse>(account)
                    };
                }

                return new BaseResponseViewModel<AccountResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountResponse>(account)
                };
            }

            //update data
            account.IsActive = false;

            reactivationAccount.DeactivateDate = Ultils.GetCurrentDatetime();
            reactivationAccount.VerifyCode = null;
            reactivationAccount.ExpirationDate = null;

            await _unitOfWork.Repository<Account>().UpdateDetached(account);
            await _unitOfWork.Repository<AccountReactivation>().UpdateDetached(reactivationAccount);
            await _unitOfWork.CommitAsync();

            if (expoToken != null)
            {
                ExpoTokenLogoutRequest token = new()
                {
                    ExpoPushToken = expoToken.Token
                };

                await Logout(token, accountId, 2);

                return new BaseResponseViewModel<AccountResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountResponse>(account)
                };
            }

            return new BaseResponseViewModel<AccountResponse>()
            {
                Status = new StatusViewModel()
                {
                    Message = "Success",
                    Success = true,
                    ErrorCode = 0
                },
                Data = _mapper.Map<AccountResponse>(account)
            };
        }

        public async Task<BaseResponseViewModel<AccountReactivationResponse>> EnableAccount(int accountId)
        {
            try
            {
                var checkAccount = _unitOfWork.Repository<Account>().GetAll()
                                                .FirstOrDefault(x => x.Id == accountId);

                if (checkAccount == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                       AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                else if (checkAccount.IsActive == true)
                {
                    throw new ErrorResponse(400, (int)AccountErrorEnums.ACCOUNT_DOES_NOT_DISABLE,
                                      AccountErrorEnums.ACCOUNT_DOES_NOT_DISABLE.GetDisplayName());
                }

                //generate random 6 digit code
                int randCode = Ultils.GenerateRandom6DigitNumber();

                //create 15 minute based on current time
                var expirationDate = Ultils.GetCurrentDatetime().AddMinutes(2);

                var reactivationAccount = _unitOfWork.Repository<AccountReactivation>().Find(x => x.AccountId == accountId);

                reactivationAccount.VerifyCode = randCode;
                reactivationAccount.ExpirationDate = expirationDate;

                //create mail content
                MailVerificationRequest content = new MailVerificationRequest()
                {
                    Email = reactivationAccount.Email,
                    Subject = "Verityfication Code",
                    Code = randCode
                };

                await _unitOfWork.Repository<AccountReactivation>().UpdateDetached(reactivationAccount);
                await _unitOfWork.CommitAsync();

                await _sendMailService.SendEmailVerification(content);

                return new BaseResponseViewModel<AccountReactivationResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountReactivationResponse>(reactivationAccount)
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AdmissionAccountResponse>> GetAccountAdmissionById(int accountId)
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
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<BaseResponseViewModel<AccountResponse>> GetAccountByEmail(string email)
        {
            try
            {
                var account = _unitOfWork.Repository<Account>().GetAll()
                                                .Where(x => x.Email.Contains(email)).FirstOrDefault();

                return new BaseResponseViewModel<AccountResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountResponse>(account)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AccountResponse>> GetAccountById(int accountId)
        {
            try
            {
                if (accountId == 0 || accountId == null)
                {
                    throw new ErrorResponse(400, (int)AccountErrorEnums.ACCOUNT_ID_NOT_NULL,
                                        AccountErrorEnums.ACCOUNT_ID_NOT_NULL.GetDisplayName());
                }

                var account = await _unitOfWork.Repository<Account>().GetAll()
                                               .Where(x => x.Id == accountId)
                                               .Include(x => x.AccountInformation)
                                               .FirstOrDefaultAsync();

                if (account == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                //check banned and update Status
                var checkAccountBanned = await CheckAccountBanned(accountId);

                if (!checkAccountBanned)
                {
                    //not banned 
                    var accountMapping = _mapper.Map<AccountResponse>(account);
                    accountMapping.IsBanned = checkAccountBanned;

                    return new BaseResponseViewModel<AccountResponse>()
                    {
                        Status = new StatusViewModel()
                        {
                            Message = "Success",
                            Success = true,
                            ErrorCode = 0
                        },
                        Data = accountMapping
                    };
                }

                var accountResponseMapping = _mapper.Map<AccountResponse>(account);
                accountResponseMapping.IsBanned = checkAccountBanned;

                return new BaseResponseViewModel<AccountResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = accountResponseMapping
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<AccountResponse>> GetAccounts(AccountResponse request, PagingRequest paging)
        {
            try
            {
                var account = _unitOfWork.Repository<Account>().GetAll()
                    .ProjectTo<AccountResponse>(_mapper.ConfigurationProvider)
                    .PagingQueryable(paging.Page, paging.PageSize);

                return new BaseResponsePagingViewModel<AccountResponse>()
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

        public async Task<BaseResponseViewModel<dynamic>> InputVerifycationCode(int accountId, int code, int roleId)
        {
            try
            {
                var inputTime = Ultils.GetCurrentDatetime();
                var checkCode = _unitOfWork.Repository<AccountReactivation>().GetAll()
                                           .FirstOrDefault(x => x.VerifyCode == code && x.ExpirationDate >= inputTime && x.AccountId == accountId);

                if (checkCode == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.VERIFY_CODE_INVALID,
                                    AccountErrorEnums.VERIFY_CODE_INVALID.GetDisplayName());
                }

                var account = _unitOfWork.Repository<Account>().Find(x => x.Id == accountId);

                account.IsActive = true;

                await _unitOfWork.Repository<Account>().UpdateDetached(account);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<dynamic>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = ""
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

                #region Identify role by email root to validate

                var identifyRootEmail = await ValidateRootEmail(userRecord.Email, (int)SystemRoleEnum.Collaborator);

                if (!identifyRootEmail)
                {
                    throw new ErrorResponse(403, (int)AccountErrorEnums.API_INVALID,
                                                AccountErrorEnums.API_INVALID.GetDisplayName());
                }

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

                    //create account information
                    await CreateAccountInformation(account.Id, account);

                    //generate token
                    var newToken = AccessTokenManager.GenerateJwtToken(string.IsNullOrEmpty(account.Name) ? "" : account.Name, account.RoleId, account.Id, _configuration);

                    //Add expo token 
                    if (data.ExpoPushToken != null && data.ExpoPushToken.Trim().Length > 0)
                        _accountExpoTokenService.AddExpoToken(data.ExpoPushToken, account.Id);

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

                else if (account.IsActive == false)
                {
                    //generate token
                    var newToken = AccessTokenManager.GenerateJwtToken(string.IsNullOrEmpty(account.Name) ? "" : account.Name, account.RoleId, account.Id, _configuration);

                    //Add fcm token     
                    if (data.ExpoPushToken != null && data.ExpoPushToken.Trim().Length > 0)
                        _accountExpoTokenService.AddExpoToken(data.ExpoPushToken, account.Id);

                    //check banned and update Status
                    var checkAccountBanned = await CheckAccountBanned(account.Id);

                    if (!checkAccountBanned)
                    {
                        //not banned 
                        var accountMapping = _mapper.Map<AccountResponse>(account);
                        accountMapping.IsBanned = checkAccountBanned;

                        return new BaseResponseViewModel<LoginResponse>()
                        {
                            Status = new StatusViewModel()
                            {
                                Message = "Account is not active",
                                Success = false,
                                ErrorCode = 4009
                            },
                            Data = new LoginResponse()
                            {
                                access_token = newToken,
                                account = accountMapping
                            }
                        };
                    }

                    var accountResponseMapping = _mapper.Map<AccountResponse>(account);
                    accountResponseMapping.IsBanned = checkAccountBanned;

                    return new BaseResponseViewModel<LoginResponse>()
                    {
                        Status = new StatusViewModel()
                        {
                            Message = "Account is not active",
                            Success = false,
                            ErrorCode = 4009
                        },
                        Data = new LoginResponse()
                        {
                            access_token = newToken,
                            account = accountResponseMapping
                        }
                    };
                }

                else
                {
                    //generate token
                    var newToken = AccessTokenManager.GenerateJwtToken(string.IsNullOrEmpty(account.Name) ? "" : account.Name, account.RoleId, account.Id, _configuration);

                    //Add fcm token     
                    if (data.ExpoPushToken != null && data.ExpoPushToken.Trim().Length > 0)
                        _accountExpoTokenService.AddExpoToken(data.ExpoPushToken, account.Id);

                    //check banned and update Status
                    var checkAccountBanned = await CheckAccountBanned(account.Id);

                    if (!checkAccountBanned)
                    {
                        //not banned 
                        var accountBannedMapping = _mapper.Map<AccountResponse>(account);
                        accountBannedMapping.IsBanned = checkAccountBanned;

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
                                account = accountBannedMapping
                            }
                        };
                    }

                    //not banned 
                    var accountSuccessMapping = _mapper.Map<AccountResponse>(account);
                    accountSuccessMapping.IsBanned = checkAccountBanned;

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
                            account = accountSuccessMapping
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task Logout(ExpoTokenLogoutRequest expoToken, int accountId, int status)
        {
            if (expoToken != null && !expoToken.ExpoPushToken.Trim().Equals("") && await _accountExpoTokenService.ValidExpoToken(expoToken.ExpoPushToken, accountId))
            {
                await _accountExpoTokenService.RemoveExpoTokens(new List<string> { expoToken.ExpoPushToken }, accountId, status);
            }
        }

        public async Task<BaseResponsePagingViewModel<AccountResponse>> SearchCollaboratorByEmail(string email, PagingRequest paging)
        {
            try
            {
                var account = _unitOfWork.Repository<Account>().GetAll()
                                         .Where(x => x.Email.Contains(email) && x.RoleId == (int)SystemRoleEnum.Collaborator && x.IsActive == true)
                                         .Include(x => x.AccountBanneds)
                                         .OrderByDescending(x => x.AccountBanneds.Max(b => b.DayEnd))
                                         .ProjectTo<AccountResponse>(_mapper.ConfigurationProvider)
                                        .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging, Constants.DefaultPaging);

                return new BaseResponsePagingViewModel<AccountResponse>()
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

        public async Task<BaseResponseViewModel<AccountResponse>> UpdateAccount(int accountId, UpdateAccountRequest request)
        {
            try
            {
                var accounts = _unitOfWork.Repository<Account>().GetAll().Where(x => x.RoleId == (int)SystemRoleEnum.Collaborator);
                var differentAccount = accounts.Where(x => x.Id != accountId);
                var collabAccount = await accounts.FirstOrDefaultAsync(x => x.Id == accountId);

                if (collabAccount == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                if (differentAccount.Any(x => x.Phone.Equals(request.Phone)))
                {
                    throw new ErrorResponse(400, (int)AccountErrorEnums.PHONE_NUM_DUPLICATE,
                                        AccountErrorEnums.PHONE_NUM_DUPLICATE.GetDisplayName());
                }

                if (collabAccount.UpdateAt.HasValue)
                {
                    if (collabAccount.UpdateAt == collabAccount.UpdateAt.Value.AddMinutes(5))
                    {
                        throw new ErrorResponse(404, (int)AccountErrorEnums.UPDATE_INVALUD,
                                            AccountErrorEnums.UPDATE_INVALUD.GetDisplayName());
                    }

                    var phoneCheck = Ultils.CheckVNPhone(request.Phone);
                    var stuIdCheck = Ultils.CheckStudentId(request.AccountInformation.IdStudent);
                    //var personalIdCheck = Ultils.CheckPersonalId(request.AccountInformation.IdentityNumber);

                    if (phoneCheck == false)
                    {
                        throw new ErrorResponse(400, (int)AccountErrorEnums.ACCOUNT_PHONE_INVALID,
                                            AccountErrorEnums.ACCOUNT_PHONE_INVALID.GetDisplayName());
                    }

                    if (stuIdCheck == false)
                    {
                        throw new ErrorResponse(400, (int)AccountErrorEnums.ACCOUNT_STUDENTID_INVALID,
                                            AccountErrorEnums.ACCOUNT_STUDENTID_INVALID.GetDisplayName());
                    }

                    //if (!string.IsNullOrEmpty(request.AccountInformation.PlaceOfIssue))
                    //{
                    //    request.AccountInformation.PlaceOfIssue = request.AccountInformation.PlaceOfIssue.ToUpper();
                    //}

                    collabAccount = _mapper.Map<UpdateAccountRequest, Account>(request, collabAccount);
                    collabAccount.UpdateAt = Ultils.GetCurrentDatetime();

                    await _unitOfWork.Repository<Account>().UpdateDetached(collabAccount);
                    await _unitOfWork.CommitAsync();

                    return new BaseResponseViewModel<AccountResponse>()
                    {
                        Status = new StatusViewModel()
                        {
                            Message = "Success",
                            Success = true,
                            ErrorCode = 0
                        },
                        Data = _mapper.Map<AccountResponse>(collabAccount)
                    };
                }

                var checkPhone = Ultils.CheckVNPhone(request.Phone);
                var checkStuId = Ultils.CheckStudentId(request.AccountInformation.IdStudent);
                //var checkPersonalId = Ultils.CheckPersonalId(request.AccountInformation.IdentityNumber);

                if (checkPhone == false)
                {
                    throw new ErrorResponse(400, (int)AccountErrorEnums.ACCOUNT_PHONE_INVALID,
                                        AccountErrorEnums.ACCOUNT_PHONE_INVALID.GetDisplayName());
                }

                if (checkStuId == false)
                {
                    throw new ErrorResponse(400, (int)AccountErrorEnums.ACCOUNT_PHONE_INVALID,
                                        AccountErrorEnums.ACCOUNT_PHONE_INVALID.GetDisplayName());
                }

                //if (!string.IsNullOrEmpty(request.AccountInformation.PlaceOfIssue))
                //{
                //    request.AccountInformation.PlaceOfIssue = request.AccountInformation.PlaceOfIssue.ToUpper();
                //}

                collabAccount = _mapper.Map<UpdateAccountRequest, Account>(request, collabAccount);

                collabAccount.UpdateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<Account>().UpdateDetached(collabAccount);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AccountResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountResponse>(collabAccount)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AccountResponse>> UpdateAccountAvatar(int accountId, UpdateAccountAvatar request)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);

                if (account == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                else if (request.ImgUrl == null || request.ImgUrl == "")
                {
                    throw new ErrorResponse(400, (int)AccountErrorEnums.ACCOUNT_AVATAR_URL_INVALID,
                                        AccountErrorEnums.ACCOUNT_AVATAR_URL_INVALID.GetDisplayName());
                }

                //DateTime updateTimeRemain = (DateTime)account.UpdateAt;

                if (account.UpdateAt.HasValue)
                {
                    if (account.UpdateAt == account.UpdateAt.Value.AddMinutes(5))
                    {
                        throw new ErrorResponse(404, (int)AccountErrorEnums.UPDATE_INVALUD,
                                            AccountErrorEnums.UPDATE_INVALUD.GetDisplayName());
                    }

                    account = _mapper.Map<UpdateAccountAvatar, Account>(request, account);

                    account.UpdateAt = Ultils.GetCurrentDatetime();

                    await _unitOfWork.Repository<Account>().UpdateDetached(account);
                    await _unitOfWork.CommitAsync();

                    return new BaseResponseViewModel<AccountResponse>()
                    {
                        Status = new StatusViewModel()
                        {
                            Message = "Success",
                            Success = true,
                            ErrorCode = 0
                        },
                        Data = _mapper.Map<AccountResponse>(account)
                    };
                }

                account = _mapper.Map<UpdateAccountAvatar, Account>(request, account);

                account.UpdateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<Account>().UpdateDetached(account);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AccountResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountResponse>(account)
                };
            }
            catch (Exception ex)
            {
                throw; 
            }
        }

        public async Task<BaseResponseViewModel<AdmissionAccountResponse>> UpdateAdmissionAccount(int accountId, UpdateAdmissionAccountRequest request)
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

                account.UpdateAt = Ultils.GetCurrentDatetime();

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

        public async Task<BaseResponseViewModel<AdmissionAccountResponse>> UpdateAdmissionAccountAvatart(int accountId, UpdateAccountAvatar request)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);

                if (account == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                else if (request.ImgUrl == null || request.ImgUrl == "")
                {
                    throw new ErrorResponse(400, (int)AccountErrorEnums.ACCOUNT_AVATAR_URL_INVALID,
                                        AccountErrorEnums.ACCOUNT_AVATAR_URL_INVALID.GetDisplayName());
                }

                //DateTime updateTimeRemain = (DateTime)account.UpdateAt;

                if (account.UpdateAt.HasValue)
                {
                    if (account.UpdateAt == account.UpdateAt.Value.AddMinutes(5))
                    {
                        throw new ErrorResponse(404, (int)AccountErrorEnums.UPDATE_INVALUD,
                                            AccountErrorEnums.UPDATE_INVALUD.GetDisplayName());
                    }

                    account = _mapper.Map<UpdateAccountAvatar, Account>(request, account);

                    account.UpdateAt = Ultils.GetCurrentDatetime();

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

                account = _mapper.Map<UpdateAccountAvatar, Account>(request, account);

                account.UpdateAt = Ultils.GetCurrentDatetime();

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
            catch
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<TotalAccountResponse>> ViewCollaborator()
        {
            try
            {
                var collaborator = _unitOfWork.Repository<Account>().GetAll()
                                              .OrderByDescending(x => x.CreateAt)
                                              .Where(x => x.RoleId == (int)SystemRoleEnum.Collaborator && x.IsActive == true);

                var newMember = collaborator.Take(10).ToList();

                var collaboratorMapper = _mapper.Map<List<NewCollaboratorResponse>>(newMember);

                TotalAccountResponse totalCollaborator = new TotalAccountResponse()
                {
                    TotalCollaborator = collaborator.Count(),
                    NewCollaborators = collaboratorMapper
                };

                return new BaseResponseViewModel<TotalAccountResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = totalCollaborator,
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<bool> CheckAccountBanned(int accountId)
        {
            try
            {
                //check banned and update Status
                var accountBanned = _unitOfWork.Repository<AccountBanned>().GetAll().Where(a => a.AccountIdBanned == accountId);

                if (!accountBanned.Any())
                {
                    return false;
                }

                var currentDateTime = Ultils.GetCurrentDatetime();

                var maxDayEnd = accountBanned.Max(x => x.DayEnd);

                if (maxDayEnd < currentDateTime)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<bool> ValidateRootEmail(string email, int roleId)
        {
            try
            {
                string[] splitEmail = email.Split('@');
                var rootEmail = splitEmail[1];

                var collabRole = await _unitOfWork.Repository<Role>().FindAsync(x => x.Id == roleId);

                if (collabRole == null || !rootEmail.Contains(collabRole.RoleEmail))
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<ManageCollabAccountResponse>> GetAllCollabAccount(int accountId, PagingRequest paging)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);
                if (!account.PostPermission)
                {
                    throw new Exceptions.ErrorResponse(401, (int)AccountReportErrorEnum.UNAUTHORIZED, AccountReportErrorEnum.UNAUTHORIZED.GetDisplayName());
                }
                var list = _unitOfWork.Repository<Account>().GetAll()
                                              .Where(x => x.Email.EndsWith("fpt.edu.vn"))
                                              .ProjectTo<ManageCollabAccountResponse>(_mapper.ConfigurationProvider)
                                              .PagingQueryable(paging.Page, paging.PageSize,
                                                               Constants.LimitPaging, Constants.DefaultPaging);

                var accountToList = list.Item2.ToList();
                foreach (var item in accountToList)
                {
                    var accountCheckBan = _unitOfWork.Repository<AccountBanned>().GetAll().Where(x => x.AccountIdBanned == item.Id);

                    if (accountCheckBan.Any(x => x.IsActive == true) && accountCheckBan.Max(x => x.DayEnd) >= Ultils.GetCurrentDatetime())
                    {
                        item.IsBanned = true;
                        item.EndTime = accountCheckBan.Max(x => x.DayEnd).ToString();
                        item.StartTime = accountCheckBan.Max(x => x.DayStart).ToString();
                    }
                }

                return new BaseResponsePagingViewModel<ManageCollabAccountResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = list.Item1
                    },
                    Data = accountToList
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region Khu vực test quét CCCD

        public async Task<BaseResponseViewModel<AccountResponse>> CreateAccountInformation(int accountId, Account account)
        {
            try
            {

                AccountInformation accountInformation = new AccountInformation()
                {
                    AccountId = accountId
                };

                await _unitOfWork.Repository<AccountInformation>().InsertAsync(accountInformation);
                await _unitOfWork.CommitAsync();

                //get account information
                accountInformation = await _unitOfWork.Repository<AccountInformation>().GetAll()
                                         .FirstOrDefaultAsync(x => x.AccountId == accountId);

                account.AccountInformationId = accountInformation.Id;

                await _unitOfWork.Repository<Account>().UpdateDetached(account);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AccountResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountResponse>(accountInformation)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AccountInformationResponse>> UpdateCitizenIdentificationFrontImg(int accountId, UpdateCitizenIdentificationFrontImg request)
        {
            try
            {
                var accountInformation = await _unitOfWork.Repository<AccountInformation>().FindAsync(x => x.AccountId == accountId);

                if (accountInformation == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                var accountInformationMapping = _mapper.Map<UpdateCitizenIdentificationFrontImg, AccountInformation>(request, accountInformation);


                await _unitOfWork.Repository<AccountInformation>().UpdateDetached(accountInformationMapping);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AccountInformationResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountInformationResponse>(accountInformationMapping)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AccountInformationResponse>> UpdateCitizenIdentificationBackImg(int accountId, UpdateCitizenIdentificationBackImg request)
        {
            try
            {
                var accountInformation = await _unitOfWork.Repository<AccountInformation>().FindAsync(x => x.AccountId == accountId);

                if (accountInformation == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                var accountInformationMapping = _mapper.Map<UpdateCitizenIdentificationBackImg, AccountInformation>(request, accountInformation);


                await _unitOfWork.Repository<AccountInformation>().UpdateDetached(accountInformationMapping);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AccountInformationResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountInformationResponse>(accountInformationMapping)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AccountInformationResponse>> UpdateCitizenIdentificationFrontImgInformation(int accountId, UpdateCitizenIdentification request)
        {
            var accountInformation = _unitOfWork.Repository<AccountInformation>().GetAll();
            var differentAccount = accountInformation.Where(x => x.AccountId != accountId);
            var accountInformationCheck = await accountInformation.FirstOrDefaultAsync(x => x.AccountId == accountId);

            if (accountInformationCheck == null)
            {
                throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                    AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
            }

            //check duplicate CCCD
            if (differentAccount.Any(x => x.IdentityNumber.Equals(request.IdentityNumber)))
            {
                throw new ErrorResponse(400, (int)AccountErrorEnums.IDENTIFICATION_DUPLICATE,
                                    AccountErrorEnums.IDENTIFICATION_DUPLICATE.GetDisplayName());
            }

            var accountInformationMapping = _mapper.Map<UpdateCitizenIdentification, AccountInformation>(request, accountInformationCheck);

            await _unitOfWork.Repository<AccountInformation>().UpdateDetached(accountInformationMapping);
            await _unitOfWork.CommitAsync();

            return new BaseResponseViewModel<AccountInformationResponse>()
            {
                Status = new StatusViewModel()
                {
                    Message = "Success",
                    Success = true,
                    ErrorCode = 0
                },
                Data = _mapper.Map<AccountInformationResponse>(accountInformationMapping)
            };
        }

        public async Task<BaseResponseViewModel<AccountInformationResponse>> UpdateCitizenIdentificationBackImgInformation(int accountId, UpdateCitizenIdentification2 request)
        {
            var accountInformation = _unitOfWork.Repository<AccountInformation>().GetAll();
            var differentAccount = accountInformation.Where(x => x.AccountId != accountId);
            var accountInformationCheck = await accountInformation.FirstOrDefaultAsync(x => x.AccountId == accountId);

            if (accountInformationCheck == null)
            {
                throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                    AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
            }

            //check duplicate CCCD
            if (differentAccount.Any(x => x.IdentityNumber.Equals(request.IdentityNumber)))
            {
                throw new ErrorResponse(400, (int)AccountErrorEnums.IDENTIFICATION_DUPLICATE,
                                    AccountErrorEnums.IDENTIFICATION_DUPLICATE.GetDisplayName());
            }

            if (!accountInformationCheck.IdentityNumber.Equals(request.IdentityNumber))
            {
                throw new ErrorResponse(400, (int)AccountErrorEnums.WRONG_BACK_CARD,
                                    AccountErrorEnums.WRONG_BACK_CARD.GetDisplayName());
            }

            var accountInformationMapping = _mapper.Map<UpdateCitizenIdentification2, AccountInformation>(request, accountInformationCheck);

            if (accountInformationMapping.PlaceOfIssue != "" || accountInformationMapping.PlaceOfIssue != null)
            {
                accountInformationMapping.PlaceOfIssue = accountInformationMapping.PlaceOfIssue.ToUpper();
            }

            await _unitOfWork.Repository<AccountInformation>().UpdateDetached(accountInformationMapping);
            await _unitOfWork.CommitAsync();

            return new BaseResponseViewModel<AccountInformationResponse>()
            {
                Status = new StatusViewModel()
                {
                    Message = "Success",
                    Success = true,
                    ErrorCode = 0
                },
                Data = _mapper.Map<AccountInformationResponse>(accountInformationMapping)
            };
        }

        public async Task<BaseResponseViewModel<AccountInformationResponse>> UpdateAccountInforamtion(int accountId, UpdateAccountInformationRequest request)
        {
            try
            {
                var accountCheck =  _unitOfWork.Repository<AccountInformation>().GetAll();
                var differentAccount = accountCheck.Where(x => x.AccountId != accountId);
                var account = await accountCheck.FirstOrDefaultAsync(x => x.AccountId == accountId);

                if (account == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }
                
                var checkStuId = Ultils.CheckStudentId(request.IdStudent);
                var checkPersonalId = Ultils.CheckPersonalId(request.IdentityNumber);

                if (checkStuId == false)
                {
                    throw new ErrorResponse(400, (int)AccountErrorEnums.ACCOUNT_STUDENTID_INVALID,
                                        AccountErrorEnums.ACCOUNT_STUDENTID_INVALID.GetDisplayName());
                }

                //check CCCD, phone number, tax number, id Student duplicate or not

                if (differentAccount.Any(x => x.IdentityNumber.Equals(request.IdentityNumber)))
                {
                    throw new ErrorResponse(400, (int)AccountErrorEnums.IDENTIFICATION_DUPLICATE,
                                        AccountErrorEnums.IDENTIFICATION_DUPLICATE.GetDisplayName());
                }

                if (differentAccount.Any(x => x.IdStudent.Equals(request.IdStudent)))
                {
                    throw new ErrorResponse(400, (int)AccountErrorEnums.STUDENT_ID_DUPLICATE,
                                        AccountErrorEnums.STUDENT_ID_DUPLICATE.GetDisplayName());
                }

                if (differentAccount.Any(x => x.TaxNumber.Equals(request.TaxNumber)))
                {
                    throw new ErrorResponse(400, (int)AccountErrorEnums.TAX_NUM_DUPLICATE,
                                        AccountErrorEnums.TAX_NUM_DUPLICATE.GetDisplayName());
                }

                if (!string.IsNullOrEmpty(request.PlaceOfIssue))
                {
                    request.PlaceOfIssue = request.PlaceOfIssue.ToUpper();
                }

                account = _mapper.Map<UpdateAccountInformationRequest, AccountInformation>(request, account);

                await _unitOfWork.Repository<AccountInformation>().UpdateDetached(account);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AccountInformationResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountInformationResponse>(account)
                };
            }
            catch (Exception ex)
            {
                //throw new ErrorResponse(500, (int)AccountErrorEnums.SERVER_BUSY,
                //                            AccountErrorEnums.SERVER_BUSY.GetDisplayName());

                throw;
            }
        }


        //test 
        public async Task<BaseResponseViewModel<AccountInformationResponse>> UpdateAccountInformationTest(int accountId, UpdateAccountInformationRequestTest request)
        {
            try
            {
                var account = await _unitOfWork.Repository<AccountInformation>().FindAsync(x => x.AccountId == accountId);

                if (account == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                //var checkStuId = Ultils.CheckStudentId(request.IdStudent);
                //var checkPersonalId = Ultils.CheckPersonalId(request.IdentityNumber);

                //if (checkStuId == false)
                //{
                //    throw new ErrorResponse(400, (int)AccountErrorEnums.ACCOUNT_STUDENTID_INVALID,
                //                        AccountErrorEnums.ACCOUNT_STUDENTID_INVALID.GetDisplayName());
                //}

                //if (!string.IsNullOrEmpty(request.PlaceOfIssue))
                //{
                //    request.PlaceOfIssue = request.PlaceOfIssue.ToUpper();
                //}

                account = _mapper.Map<UpdateAccountInformationRequestTest, AccountInformation>(request, account);

                await _unitOfWork.Repository<AccountInformation>().UpdateDetached(account);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AccountInformationResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountInformationResponse>(account)
                };
            }
            catch (Exception ex)
            {
                //throw new ErrorResponse(500, (int)AccountErrorEnums.SERVER_BUSY,
                //                            AccountErrorEnums.SERVER_BUSY.GetDisplayName());

                throw;
            }
        }

        public async Task<BaseResponseViewModel<AccountResponse>> UpdateCollaboratorCredential(int accountId, int collaboratorAccountId)
        {
            try
            {
                var admissionAccount = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId && x.RoleId == (int)SystemRoleEnum.AdmissionManager);

                if (admissionAccount == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                       AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                if (admissionAccount.PostPermission == false)
                {
                    throw new ErrorResponse(403, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                                       AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                var collaboratorAccount = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == collaboratorAccountId && x.RoleId == (int)SystemRoleEnum.Collaborator);

                if (collaboratorAccount == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.COLLABORATOR_NOT_FOUND,
                                       AccountErrorEnums.COLLABORATOR_NOT_FOUND.GetDisplayName());
                }

                collaboratorAccount.IsPremium = true;
                collaboratorAccount.UpdateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<Account>().UpdateDetached(collaboratorAccount);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AccountResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountResponse>(collaboratorAccount)
                };

            }
            catch(Exception ex)
            {
                throw;
            }
        }

        #endregion
    }
}
