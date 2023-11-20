using AutoMapper;
using AutoMapper.QueryableExtensions;
using LAK.Sdk.Core.Utilities;
using Microsoft.EntityFrameworkCore;
using SupFAmof.Data.Entity;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Admin;
using SupFAmof.Service.DTO.Request.Staff;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admin;
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
    public class ManageAdmissionProfileService : IManageAdmissionProfileService
    {
        #region define service in use

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ManageAdmissionProfileService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        #endregion

        public async Task<BaseResponsePagingViewModel<AdminAccountAdmissionResponse>> GetAdmissionAccount(int adminAccountId, AdminAccountAdmissionResponse filter, PagingRequest paging)
        {
            try
            {
                //check account
                var checkAdminAccount = await _unitOfWork.Repository<Admin>().GetAll().FirstOrDefaultAsync(x => x.Id == adminAccountId && x.IsAvailable == true);

                if (checkAdminAccount == null)
                {
                    //account is not avalable to use
                    throw new ErrorResponse(403, (int)AdminAccountErrorEnum.ADMIN_FORBIDDEN,
                                        AdminAccountErrorEnum.ADMIN_FORBIDDEN.GetDisplayName());
                }

                var admissionAccounts = _unitOfWork.Repository<Account>().GetAll()
                                                  .Where(x => x.RoleId == (int)SystemRoleEnum.AdmissionManager)
                                                  .OrderByDescending(x => x.CreateAt)
                                                  .ProjectTo<AdminAccountAdmissionResponse>(_mapper.ConfigurationProvider)
                                                  .DynamicFilter(filter)
                                                  .DynamicSort(paging.Sort, paging.Order)
                                                  .PagingQueryable(paging.Page, paging.PageSize);

                return new BaseResponsePagingViewModel<AdminAccountAdmissionResponse>
                {
                    Metadata = new PagingsMetadata
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = admissionAccounts.Item1
                    },
                    Data = admissionAccounts.Item2.ToList(),
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<AdminAccountAdmissionResponse>> SearchAdmissionAccount(int adminAccountId, string search, PagingRequest paging)
        {
            try
            {
                //check account
                var checkAdminAccount = await _unitOfWork.Repository<Admin>().GetAll().FirstOrDefaultAsync(x => x.Id == adminAccountId && x.IsAvailable == true);

                if (checkAdminAccount == null)
                {
                    //account is not avalable to use
                    throw new ErrorResponse(403, (int)AdminAccountErrorEnum.ADMIN_FORBIDDEN,
                                        AdminAccountErrorEnum.ADMIN_FORBIDDEN.GetDisplayName());
                }

                var accounts = _unitOfWork.Repository<Account>()
                                              .GetAll()
                                              .Where(x => x.Email.Contains(search) && x.RoleId == (int)SystemRoleEnum.AdmissionManager)
                                              .ProjectTo< AdminAccountAdmissionResponse>(_mapper.ConfigurationProvider)
                                              .DynamicSort(paging.Sort, paging.Order)
                                              .PagingQueryable(paging.Page, paging.PageSize);

                return new BaseResponsePagingViewModel<AdminAccountAdmissionResponse>
                {
                    Metadata = new PagingsMetadata
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = accounts.Item1
                    },
                    Data = await accounts.Item2.ToListAsync(),
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AdminAccountAdmissionResponse>> UpdateAdmissionCertificate(int adminAccountId, UpdateAdminAccountAdmissionRequest request)
        {
            try
            {
                //check account
                var checkAdminAccount = await _unitOfWork.Repository<Admin>().GetAll().FirstOrDefaultAsync(x => x.Id == adminAccountId && x.IsAvailable == true);

                if (checkAdminAccount == null)
                {
                    //account is not avalable to use
                    throw new ErrorResponse(403, (int)AdminAccountErrorEnum.ADMIN_FORBIDDEN,
                                        AdminAccountErrorEnum.ADMIN_FORBIDDEN.GetDisplayName());
                }

                var checkAdmisison = await _unitOfWork.Repository<Account>()
                                                .FindAsync(x => x.Id == request.Id && x.RoleId == (int)SystemRoleEnum.AdmissionManager);

                if (checkAdmisison == null)
                {
                    throw new ErrorResponse(404, (int)AccountErrorEnums.ACCOUNT_NOT_FOUND,
                                        AccountErrorEnums.ACCOUNT_NOT_FOUND.GetDisplayName());
                }

                var result = _mapper.Map<UpdateAdminAccountAdmissionRequest, Account>(request, checkAdmisison);

                result.UpdateAt = Ultils.GetCurrentDatetime();

                return new BaseResponseViewModel<AdminAccountAdmissionResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdminAccountAdmissionResponse>(result)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
