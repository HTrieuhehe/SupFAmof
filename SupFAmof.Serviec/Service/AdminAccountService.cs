using SupFAmof.Service.DTO.Request.Staff;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using AutoMapper;
using SupFAmof.Data.UnitOfWork;
using Microsoft.Extensions.Configuration;
using SupFAmof.Data.Entity;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Utilities;
using AutoMapper.QueryableExtensions;
using Service.Commons;
using SupFAmof.Service.Helpers;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.ErrorEnum;
using SupFAmof.Service.DTO.Response.Admin;
using LAK.Sdk.Core.Utilities;

namespace SupFAmof.Service.Service
{
    public class AdminAccountService : IAdminAccountService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IFcmTokenService _fcmTokenService;
        private readonly string _supFAmof;

        public AdminAccountService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration, IFcmTokenService fcmTokenService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _fcmTokenService = fcmTokenService;
            _supFAmof = _configuration["supfamof2023"];
        }

        public async Task<BaseResponseViewModel<AdminAccountResponse>> CreateAdminManager(CreateAdminAccountRequest request)
        {
            var checkAdmin = _unitOfWork.Repository<Admin>()
                                        .Find(x => x.Username.Contains(request.Username));

            if (checkAdmin != null)
            {
                throw new ErrorResponse(400, (int)AdminAccountErrorEnum.ADMIN_EXSIST,
                                    AdminAccountErrorEnum.ADMIN_EXSIST.GetDisplayName());
            }
            var admin = _mapper.Map<CreateAdminAccountRequest, Admin>(request);

            //convert from string to byte
            admin.Password = null;
            admin.Password = Ultils.GetHash(request.Password, _supFAmof);
            admin.IsAvailable = true;
            admin.CreateAt = DateTime.Now;

            await _unitOfWork.Repository<Admin>().InsertAsync(admin);
            await _unitOfWork.CommitAsync();

            return new BaseResponseViewModel<AdminAccountResponse>()
            {
                Status = new StatusViewModel()
                {
                    Message = "Success",
                    Success = true,
                    ErrorCode = 0
                },
                Data = _mapper.Map<AdminAccountResponse>(admin)
            };
        }

        public async Task<BaseResponseViewModel<AdminAccountResponse>> GetAdminById(int adminId)
        {
            var admin = _unitOfWork.Repository<Admin>().GetAll()
                                   .FirstOrDefault(x => x.Id == adminId);
            if (admin == null)
            {
                throw new ErrorResponse(404, (int)AdminAccountErrorEnum.NOT_FOUND_ID,
                                    AdminAccountErrorEnum.NOT_FOUND_ID.GetDisplayName());
            }
            return new BaseResponseViewModel<AdminAccountResponse>()
            {
                Status = new StatusViewModel()
                {
                    Message = "Success",
                    Success = true,
                    ErrorCode = 0
                },
                Data = _mapper.Map<AdminAccountResponse>(admin)
            };
        }

        public async Task<BaseResponsePagingViewModel<AdminAccountResponse>> GetAdmins(AdminAccountResponse filter, PagingRequest paging)
        {
            var staff = _unitOfWork.Repository<Admin>().GetAll()
                                   .ProjectTo<AdminAccountResponse>(_mapper.ConfigurationProvider)
                                   .DynamicFilter(filter)
                                   .DynamicSort(paging.Sort, paging.Order)
                                   .PagingQueryable(paging.Page, paging.PageSize);
            return new BaseResponsePagingViewModel<AdminAccountResponse>()
            {
                Metadata = new PagingsMetadata()
                {
                    Page = paging.Page,
                    Size = paging.PageSize,
                    Total = staff.Item1
                },
                Data = staff.Item2.ToList()
            };
        }

        public async Task<BaseResponseViewModel<dynamic>> Login(LoginRequest request)
        {
            var staff = _unitOfWork.Repository<Admin>().GetAll()
                                   .Where(x => x.Username.Equals(request.Username) && x.IsAvailable == true)
                                   .FirstOrDefault();

            //convert Login Password and check with database
            if (staff == null || !Ultils.CompareHash(request.Password, staff.Password, _supFAmof))
            {
                throw new ErrorResponse(400, (int)AdminAccountErrorEnum.LOGIN_FAIL,
                                    AdminAccountErrorEnum.LOGIN_FAIL.GetDisplayName());
            }

            //Login success
            //continue
            //if (request.FcmToken != null && request.FcmToken.Trim().Length > 0)
            //    _fcmTokenService.AddStaffFcmToken(request.FcmToken, staff.Id);

            var token = AccessTokenManager.GenerateJwtToken(staff.Name, (int)SystemRoleEnum.SystemAdmin , staff.Id, _configuration);
            return new BaseResponseViewModel<dynamic>()
            {
                Status = new StatusViewModel()
                {
                    Success = true,
                    Message = "Success",
                    ErrorCode = 0
                },
                Data = new
                {
                    Id = staff.Id,
                    Username = staff.Username,
                    Name = staff.Name,
                    AccessToken = token
                }
            };
        }

        public async Task<BaseResponseViewModel<AdminAccountResponse>> UpdateAdmin(int adminId, UpdateAdminAccountRequest request)
        {
            var admin = _unitOfWork.Repository<Admin>().Find(x => x.Id == adminId);
            if (admin == null)
            {
                throw new ErrorResponse(404, (int)AdminAccountErrorEnum.NOT_FOUND_ID,
                                    AdminAccountErrorEnum.NOT_FOUND_ID.GetDisplayName());
            }

            var adminMappingResult = _mapper.Map<UpdateAdminAccountRequest, Admin>(request, admin);

            //nếu password mới được đưa vào thì phải chuyển lại thành byte với nhưng thông tin được cung cấp kèm theo
            if(request.Password != null)
            {
                admin.Password = null;
                admin.Password = Ultils.GetHash(request.Password, _supFAmof);

                adminMappingResult.UpdateAt = DateTime.Now;
                await _unitOfWork.Repository<Admin>()
                                .UpdateDetached(adminMappingResult);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<AdminAccountResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdminAccountResponse>(adminMappingResult)
                };
            }

            adminMappingResult.UpdateAt = DateTime.Now;
            await _unitOfWork.Repository<Admin>()
                            .UpdateDetached(adminMappingResult);
            await _unitOfWork.CommitAsync();
            return new BaseResponseViewModel<AdminAccountResponse>()
            {
                Status = new StatusViewModel()
                {
                    Message = "Success",
                    Success = true,
                    ErrorCode = 0
                },
                Data = _mapper.Map<AdminAccountResponse>(adminMappingResult)
            };
        }
    }
}
