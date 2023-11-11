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
using static SupFAmof.Service.Helpers.ErrorEnum;

namespace SupFAmof.Service.Service
{
    public class AttendanceService : IAttendanceService
    {
        //chỉ có những post có status close hoặc end thì mới cho phép coi attendance

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AttendanceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        #region Admission

        public async Task<BaseResponsePagingViewModel<AdmissionAttendanceResponse>> GetAttendanceHistoryByPositionId
            (int accountId, int? positionId, PagingRequest paging)
        
        { 
            try
            {
                if (positionId == null|| positionId == 0)
                {
                    throw new ErrorResponse(404, (int)CheckAttendanceErrorEnum.ATTENDANCE_NOT_FOUND,
                                        CheckAttendanceErrorEnum.ATTENDANCE_NOT_FOUND.GetDisplayName());
                }

                //check account post Permission
                var account = await _unitOfWork.Repository<Account>().FindAsync(x => x.Id == accountId);

                if (account == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                else if (account.PostPermission == false)
                {
                    throw new ErrorResponse(403, (int)AccountErrorEnums.PERMISSION_NOT_ALLOW,
                                        AccountErrorEnums.PERMISSION_NOT_ALLOW.GetDisplayName());
                }

                //view attendance
                var attendanceHistory = _unitOfWork.Repository<CheckAttendance>().GetAll()
                                                   .Where(x => x.PostRegistration.PositionId == positionId)
                                                   .ProjectTo<AdmissionAttendanceResponse>(_mapper.ConfigurationProvider)
                                                   .PagingQueryable(paging.Page, paging.PageSize);


                return new BaseResponsePagingViewModel<AdmissionAttendanceResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = attendanceHistory.Item1
                    },
                    Data = attendanceHistory.Item2.ToList(),
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
