using AutoMapper;
using SupFAmof.Data.Entity;
using SupFAmof.Data.UnitOfWork;
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
    public class AdmissionCredentiaService : IAdmissionCredentialService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public AdmissionCredentiaService(IUnitOfWork unitOfWork, INotificationService notificationService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _mapper = mapper;   
        }

        public async Task<BaseResponseViewModel<AccountResponse>> CreateAdmissionCredential(int administratorId, int accountId)
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

                return new BaseResponseViewModel<AccountResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountResponse>(admission)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AccountResponse>> DisableAdmissionCredential(int administratorId, int accountId)
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

                return new BaseResponseViewModel<AccountResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AccountResponse>(admission)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
