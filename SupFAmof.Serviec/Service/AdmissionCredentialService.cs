using AutoMapper;
using AutoMapper.QueryableExtensions;
using LAK.Sdk.Core.Utilities;
using Microsoft.EntityFrameworkCore;
using SupFAmof.Data.Entity;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Service.ServiceInterface;
using SupFAmof.Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SupFAmof.Service.Helpers.Enum;
using static SupFAmof.Service.Helpers.ErrorEnum;

namespace SupFAmof.Service.Service
{
    public class AdmissionCredentialService : IAdmissionCredentialService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public AdmissionCredentialService(IUnitOfWork unitOfWork, INotificationService notificationService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _mapper = mapper;   
        }

        public async Task<BaseResponseViewModel<AdmissionAccountResponse>> CreateAdmissionCredential(int administratorId, int accountId)
        {
            try
            {
                //check administrator
                var admin = await _unitOfWork.Repository<Admin>().FindAsync(x => x.Id == administratorId);

                if (admin == null)
                {
                    throw new ErrorResponse(404, (int)AdminErrorEnums.ADMIN_NOT_FOUND,
                                        AdminErrorEnums.ADMIN_NOT_FOUND.GetDisplayName());
                }

                //check admission
                var admission = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId && x.RoleId == (int)SystemRoleEnum.AdmissionManager);

                if (admission == null)
                {
                    throw new ErrorResponse(404, (int)AdmissionCredentialErrorEnums.ALREADY_UPGRADE,
                                        AdmissionCredentialErrorEnums.ALREADY_UPGRADE.GetDisplayName());
                }

                if (admission.PostPermission == true)
                {
                    throw new ErrorResponse(404, (int)AdmissionCredentialErrorEnums.ALREADY_UPGRADE,
                                       AdmissionCredentialErrorEnums.ALREADY_UPGRADE.GetDisplayName());
                }

                admission.IsPremium = true;
                admission.UpdateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<Account>().UpdateDetached(admission);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AdmissionAccountResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdmissionAccountResponse>(admission)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AdmissionAccountResponse>> DisableAdmissionCredential(int administratorId, int accountId)
        {
            try
            {
                //check administrator
                var admin = await _unitOfWork.Repository<Admin>().FindAsync(x => x.Id == administratorId);

                if (admin == null)
                {
                    throw new ErrorResponse(404, (int)AdminErrorEnums.ADMIN_NOT_FOUND,
                                        AdminErrorEnums.ADMIN_NOT_FOUND.GetDisplayName());
                }

                //check admission
                var admission = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId && x.RoleId == (int)SystemRoleEnum.AdmissionManager);

                if (admission == null)
                {
                    throw new ErrorResponse(404, (int)AdmissionCredentialErrorEnums.ALREADY_UPGRADE,
                                        AdmissionCredentialErrorEnums.ALREADY_UPGRADE.GetDisplayName());
                }

                if (admission.PostPermission == false)
                {
                    throw new ErrorResponse(404, (int)AdmissionCredentialErrorEnums.ALREADY_DISABLE,
                                       AdmissionCredentialErrorEnums.ALREADY_DISABLE.GetDisplayName());
                }

                admission.IsPremium = false;
                admission.UpdateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<Account>().UpdateDetached(admission);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AdmissionAccountResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdmissionAccountResponse>(admission)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<AdmissionAccountResponse>> GetAdmissionProfile(int administratorId, AdmissionAccountResponse filter, PagingRequest paging)
        {
            try
            {
                //check administrator
                var admin = await _unitOfWork.Repository<Admin>().FindAsync(x => x.Id == administratorId);

                if (admin == null)
                {
                    throw new ErrorResponse(404, (int)AdminErrorEnums.ADMIN_NOT_FOUND,
                                        AdminErrorEnums.ADMIN_NOT_FOUND.GetDisplayName());
                }

                //get admission
                var admissionProfiles = _unitOfWork.Repository<Account>().GetAll()
                                                .ProjectTo<AdmissionAccountResponse>(_mapper.ConfigurationProvider)
                                                .Where(x => x.RoleId == (int)SystemRoleEnum.AdmissionManager)
                                                .DynamicFilter(filter)
                                                .DynamicSort(paging.Sort, paging.Order)
                                                .PagingQueryable(paging.Page, paging.PageSize);

                return new BaseResponsePagingViewModel<AdmissionAccountResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = admissionProfiles.Item1
                    },
                    Data = await admissionProfiles.Item2.ToListAsync()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
