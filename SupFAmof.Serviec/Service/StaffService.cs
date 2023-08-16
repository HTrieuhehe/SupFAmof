using SupFAmof.Service.DTO.Request.Staff;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using AutoMapper;
using SupFAmof.Data.UnitOfWork;
using Microsoft.Extensions.Configuration;
using SupFAmof.Data.Entity;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.Exceptions;
using NTQ.Sdk.Core.Utilities;
using SupFAmof.Service.Utilities;
using AutoMapper.QueryableExtensions;
using Service.Commons;
using SupFAmof.Service.Helpers;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.ErrorEnum;

namespace SupFAmof.Service.Service
{
    public class StaffService : IStaffService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IFcmTokenService _fcmTokenService;
        private readonly string _supFAmof;

        public StaffService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration configuration, IFcmTokenService fcmTokenService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _fcmTokenService = fcmTokenService;
            _supFAmof = _configuration["supfamof2023"];
        }

        public async Task<BaseResponseViewModel<StaffResponse>> CreateAdminManager(CreateStaffRequest request)
        {
            var checkStaff = _unitOfWork.Repository<staff>()
                                        .Find(x => x.Username.Contains(request.Username));

            if (checkStaff != null)
            {
                throw new ErrorResponse(404, (int)StaffErrorEnum.STAFF_EXSIST,
                                    StaffErrorEnum.STAFF_EXSIST.GetDisplayName());
            }
            var staff = _mapper.Map<CreateStaffRequest, staff>(request);

            //convert from string to byte
            staff.Password = null;
            staff.Password = Ultils.GetHash(request.Password, _supFAmof);
            staff.IsAvailable = true;
            staff.CreateAt = DateTime.Now;

            await _unitOfWork.Repository<staff>().InsertAsync(staff);
            await _unitOfWork.CommitAsync();

            return new BaseResponseViewModel<StaffResponse>()
            {
                Status = new StatusViewModel()
                {
                    Message = "Success",
                    Success = true,
                    ErrorCode = 0
                },
                Data = _mapper.Map<StaffResponse>(staff)
            };
        }

        public async Task<BaseResponseViewModel<StaffResponse>> GetStaffById(int staffId)
        {
            var staff = _unitOfWork.Repository<staff>().GetAll()
                                   .FirstOrDefault(x => x.Id == staffId);
            if (staff == null)
            {
                throw new ErrorResponse(404, (int)StaffErrorEnum.NOT_FOUND_ID,
                                    StaffErrorEnum.NOT_FOUND_ID.GetDisplayName());
            }
            return new BaseResponseViewModel<StaffResponse>()
            {
                Status = new StatusViewModel()
                {
                    Message = "Success",
                    Success = true,
                    ErrorCode = 0
                },
                Data = _mapper.Map<StaffResponse>(staff)
            };
        }

        public async Task<BaseResponsePagingViewModel<StaffResponse>> GetStaffs(StaffResponse filter, PagingRequest paging)
        {
            var staff = _unitOfWork.Repository<staff>().GetAll()
                                   .ProjectTo<StaffResponse>(_mapper.ConfigurationProvider)
                                   .DynamicFilter(filter)
                                   .DynamicSort(filter)
                                   .PagingQueryable(paging.Page, paging.PageSize, Constants.LimitPaging,
                                    Constants.DefaultPaging);
            return new BaseResponsePagingViewModel<StaffResponse>()
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
            var staff = _unitOfWork.Repository<staff>().GetAll()
                                   .Where(x => x.Username.Equals(request.Username) && x.IsAvailable == true)
                                   .FirstOrDefault();

            //convert Login Password and check with database
            if (staff == null || !Ultils.CompareHash(request.Password, staff.Password, _supFAmof))
            {
                return new BaseResponseViewModel<dynamic>()
                {
                    Status = new StatusViewModel()
                    {
                        Success = false,
                        Message = StaffErrorEnum.LOGIN_FAIL.GetDisplayName(),
                        ErrorCode = (int)StaffErrorEnum.LOGIN_FAIL
                    }
                };
            }

            //Login success
            //continue
            if (request.FcmToken != null && request.FcmToken.Trim().Length > 0)
                _fcmTokenService.AddStaffFcmToken(request.FcmToken, staff.Id);

            var token = AccessTokenManager.GenerateJwtToken(staff.Name, 0, staff.Id, _configuration);
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

        public async Task<BaseResponseViewModel<StaffResponse>> UpdateStaff(int staffId, UpdateStaffRequest request)
        {
            var staff = _unitOfWork.Repository<staff>().Find(x => x.Id == staffId);
            if (staff == null)
            {
                throw new ErrorResponse(404, (int)StaffErrorEnum.NOT_FOUND_ID,
                                    StaffErrorEnum.NOT_FOUND_ID.GetDisplayName());
            }

            var staffMappingResult = _mapper.Map<UpdateStaffRequest, staff>(request, staff);

            //nếu password mới được đưa vào thì phải chuyển lại thành byte với nhưng thông tin được cung cấp kèm theo
            if(request.Password != null)
            {
                staff.Password = null;
                staff.Password = Ultils.GetHash(request.Password, _supFAmof);

                staffMappingResult.UpdateAt = DateTime.Now;
                await _unitOfWork.Repository<staff>()
                                .UpdateDetached(staffMappingResult);
                await _unitOfWork.CommitAsync();
                return new BaseResponseViewModel<StaffResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<StaffResponse>(staffMappingResult)
                };
            }

            staffMappingResult.UpdateAt = DateTime.Now;
            await _unitOfWork.Repository<staff>()
                            .UpdateDetached(staffMappingResult);
            await _unitOfWork.CommitAsync();
            return new BaseResponseViewModel<StaffResponse>()
            {
                Status = new StatusViewModel()
                {
                    Message = "Success",
                    Success = true,
                    ErrorCode = 0
                },
                Data = _mapper.Map<StaffResponse>(staffMappingResult)
            };
        }
    }
}
