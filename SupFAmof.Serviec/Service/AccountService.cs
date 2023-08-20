using AutoMapper;
using System.Data;
using Service.Commons;
using FirebaseAdmin.Auth;
using SupFAmof.Data.Entity;
using NTQ.Sdk.Core.Utilities;
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

namespace SupFAmof.Service.Service
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IFcmTokenService _accountFcmtokenService;

        public AccountService
            (IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, IFcmTokenService accountFcmtokenService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _accountFcmtokenService = accountFcmtokenService;
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

        public async Task<BaseResponseViewModel<AccountResponse>> CreateAccountInformation(int accountId, CreateAccountInformationRequest request)
        {
            try
            {
                var account = _unitOfWork.Repository<Account>().GetAll()
                                        .FirstOrDefault(x => x.Id == accountId);

                if(account == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                                          AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                //check account Information

                var accountInfoCheck = _unitOfWork.Repository<AccountInformation>().GetAll().FirstOrDefault(x => x.AccountId == accountId);

                if (accountInfoCheck != null)
                {
                    throw new ErrorResponse(400, (int)AccountErrorEnums.ACCOUNT_INFOMRATION_EXISTED,
                                                          AccountErrorEnums.ACCOUNT_INFOMRATION_EXISTED.GetDisplayName());
                }

                var checkStuId = Ultils.CheckStudentId(request.IdStudent);
                var checkPersonalId = Ultils.CheckPersonalId(request.PersonalId);

                if (checkPersonalId == false)
                {
                    throw new ErrorResponse(400, (int)AccountErrorEnums.ACCOUNT_INVALID_PERSONAL_ID,
                                        AccountErrorEnums.ACCOUNT_INVALID_PERSONAL_ID.GetDisplayName());
                }

                if (checkStuId == false)
                {
                    throw new ErrorResponse(400, (int)AccountErrorEnums.ACCOUNT_PHONE_INVALID,
                                        AccountErrorEnums.ACCOUNT_PHONE_INVALID.GetDisplayName());
                }

                var accountInfo = _mapper.Map<AccountInformation>(request);

                accountInfo.AccountId = account.Id;
                accountInfo.PlaceOfIssue = accountInfo.PlaceOfIssue.ToUpper();

                await _unitOfWork.Repository<AccountInformation>().InsertAsync(accountInfo);
                await _unitOfWork.CommitAsync();

                //update AccountInformationId to Account Table
                var updateAccount = _unitOfWork.Repository<AccountInformation>().GetAll().FirstOrDefault(x => x.AccountId == accountId);

                account.AccountInformationId = updateAccount.Id;

                await _unitOfWork.Repository<AccountInformation>().UpdateDetached(updateAccount);
                await _unitOfWork.CommitAsync();

                //response new Infomation
                var finalInfo = _unitOfWork.Repository<Account>().GetAll()
                                       .FirstOrDefault(x => x.Id == accountId);

                var result = _mapper.Map<AccountResponse>(finalInfo);

                return new BaseResponseViewModel<AccountResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountResponse>(result)
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

            if (account == null)
            {
                throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                    AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
            }

            account.UpdateAt = DateTime.Now;
            account.IsActive = false;

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
                var account = await _unitOfWork.Repository<Account>().GetAll()
                                               .Where(x => x.Id == accountId)
                                               .FirstOrDefaultAsync();

                if (account == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                #region Calculating Post and Salary

                var postRegistrations = _unitOfWork.Repository<PostRegistration>().GetAll()
                                      .Include(x => x.PostRegistrationDetails)
                                      .Where(x => x.AccountId == accountId && x.Status == (int)PostRegistrationStatusEnum.Confirm)
                                      .ToList();
                int totalPost = 0;
                double totalSalary = 0;

                if (postRegistrations.Any())
                {
                    totalPost = postRegistrations.Count();
                    foreach (var postRegistration in postRegistrations)
                    {
                        foreach (var detail in postRegistration.PostRegistrationDetails)
                        {
                            totalSalary += detail.FinalSalary;
                        }
                    }

                    var accountResponse = _mapper.Map<AccountResponse>(account);

                    accountResponse.AccountMonthlyReport.TotalPost = totalPost;
                    accountResponse.AccountMonthlyReport.TotalSalary = totalSalary;

                    #endregion

                    return new BaseResponseViewModel<AccountResponse>()
                    {
                        Status = new StatusViewModel()
                        {
                            Message = "Success",
                            Success = true,
                            ErrorCode = 0
                        },
                        Data = accountResponse
                    };
                }

                var accountResponseMapping = _mapper.Map<AccountResponse>(account);

                accountResponseMapping.AccountMonthlyReport.TotalPost = totalPost;
                accountResponseMapping.AccountMonthlyReport.TotalSalary = totalSalary;

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

        public async Task<BaseResponseViewModel<AccountResponse>> UpdateAccount(int accountId, UpdateAccountRequest request)
        {
            try
            {
                Account account = _unitOfWork.Repository<Account>()
                                         .Find(x => x.Id == accountId);

                if(account == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }


                var checkPhone = Ultils.CheckVNPhone(request.Phone);
                var checkStuId = Ultils.CheckStudentId(request.UpdateAccountInformation.IdStudent);
                var checkPersonalId = Ultils.CheckPersonalId(request.UpdateAccountInformation.PersonalId);

                if(checkPhone == false)
                {
                    throw new ErrorResponse(400, (int)AccountErrorEnums.ACCOUNT_PHONE_INVALID,
                                        AccountErrorEnums.ACCOUNT_PHONE_INVALID.GetDisplayName());
                }

                if (checkStuId == false)
                {
                    throw new ErrorResponse(400, (int)AccountErrorEnums.ACCOUNT_PHONE_INVALID,
                                        AccountErrorEnums.ACCOUNT_PHONE_INVALID.GetDisplayName());
                }

                if (!string.IsNullOrEmpty(request.UpdateAccountInformation.PlaceOfIssue))
                {
                    request.UpdateAccountInformation.PlaceOfIssue = request.UpdateAccountInformation.PlaceOfIssue.ToUpper();
                }

                account = _mapper.Map<UpdateAccountRequest, Account>(request, account);
                
                account.UpdateAt = DateTime.Now;

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
            catch(Exception ex)
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
