using AutoMapper;
using AutoMapper.QueryableExtensions;
using DocumentFormat.OpenXml.VariantTypes;
using LAK.Sdk.Core.Utilities;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using SupFAmof.Data.Entity;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Admin;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admin;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Service.ServiceInterface;
using SupFAmof.Service.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SupFAmof.Service.Helpers.ErrorEnum;

namespace SupFAmof.Service.Service
{
    public class SystemManagementService : ISystemManagementService
    {
        #region define service in use

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public SystemManagementService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        #endregion

        public async Task<BaseResponseViewModel<AdminSystemManagementResponse>> CreateRole(int accountId, CreateAdminSystemManagementRequest request)
        {
            try
            {
                //check account
                var checkAdminAccount = await _unitOfWork.Repository<Admin>().GetAll().FirstOrDefaultAsync(x => x.Id == accountId && x.IsAvailable == true);

                if (checkAdminAccount == null)
                {
                    //account is not avalable to use
                    throw new ErrorResponse(403, (int)AdminAccountErrorEnum.ADMIN_FORBIDDEN,
                                        AdminAccountErrorEnum.ADMIN_FORBIDDEN.GetDisplayName());
                }

                //validate if the role email is containing @ character
                if(request.RoleEmail.Contains('@'))
                {
                    throw new ErrorResponse(400, (int)AdminSystemManagementErrorEnum.CHARACTER_INVALID,
                                                            AdminSystemManagementErrorEnum.CHARACTER_INVALID.GetDisplayName());
                }

                var role = _mapper.Map<CreateAdminSystemManagementRequest, Role>(request);
                role.IsActive = true;
                role.CreateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<Role>().InsertAsync(role);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AdminSystemManagementResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdminSystemManagementResponse>(role)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AdminSystemManagementResponse>> DisableRole(int accountId, int roleId)
        {
            try
            {
                //check account
                var checkAdminAccount = await _unitOfWork.Repository<Admin>().GetAll().FirstOrDefaultAsync(x => x.Id == accountId && x.IsAvailable == true);

                if (checkAdminAccount == null)
                {
                    //account is not avalable to use
                    throw new ErrorResponse(403, (int)AdminAccountErrorEnum.ADMIN_FORBIDDEN,
                                        AdminAccountErrorEnum.ADMIN_FORBIDDEN.GetDisplayName());
                }

                var role = await _unitOfWork.Repository<Role>().FindAsync(x => x.Id == roleId);

                if (role == null)
                {
                    throw new ErrorResponse(404, (int)AdminSystemManagementErrorEnum.SYSTEM_ROLE_NOT_FOUND,
                                                            AdminSystemManagementErrorEnum.SYSTEM_ROLE_NOT_FOUND.GetDisplayName());
                }

                role.IsActive = false;
                role.UpdateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<Role>().UpdateDetached(role);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AdminSystemManagementResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdminSystemManagementResponse>(role)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AdminSystemManagementResponse>> GetRoleById(int accountId, int roleId)
        {
            try
            {
                //check account
                var checkAdminAccount = await _unitOfWork.Repository<Admin>().GetAll().FirstOrDefaultAsync(x => x.Id == accountId && x.IsAvailable == true);

                if (checkAdminAccount == null)
                {
                    //account is not avalable to use
                    throw new ErrorResponse(403, (int)AdminAccountErrorEnum.ADMIN_FORBIDDEN,
                                        AdminAccountErrorEnum.ADMIN_FORBIDDEN.GetDisplayName());
                }

                var role = _unitOfWork.Repository<Role>().FindAsync(x => x.Id == roleId);

                if (role == null)
                {
                    throw new ErrorResponse(404, (int)AdminSystemManagementErrorEnum.SYSTEM_ROLE_NOT_FOUND,
                                                            AdminSystemManagementErrorEnum.SYSTEM_ROLE_NOT_FOUND.GetDisplayName());
                }

                return new BaseResponseViewModel<AdminSystemManagementResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdminSystemManagementResponse>(role)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponsePagingViewModel<AdminSystemManagementResponse>> GetRoles(int accountId, AdminSystemManagementResponse filter, PagingRequest paging)
        {
            try
            {
                //check account
                var checkAdminAccount = await _unitOfWork.Repository<Admin>().GetAll().FirstOrDefaultAsync(x => x.Id == accountId && x.IsAvailable == true);

                if (checkAdminAccount == null)
                {
                    //account is not avalable to use
                    throw new ErrorResponse(403, (int)AdminAccountErrorEnum.ADMIN_FORBIDDEN,
                                        AdminAccountErrorEnum.ADMIN_FORBIDDEN.GetDisplayName());
                }

                var roles = _unitOfWork.Repository<Role>().GetAll()
                                       .Where(x => x.IsActive == true)
                                       .ProjectTo<AdminSystemManagementResponse>(_mapper.ConfigurationProvider)
                                       .DynamicFilter(filter)
                                       .DynamicSort(paging.Sort, paging.Order)
                                       .PagingQueryable(paging.Page, paging.PageSize);

                return new BaseResponsePagingViewModel<AdminSystemManagementResponse>
                {
                    Metadata = new PagingsMetadata
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = roles.Item1
                    },
                    Data = roles.Item2.ToList(),
                };
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<BaseResponseViewModel<AdminSystemManagementResponse>> UpdateRole(int accountId, int roleId, UpdateAdminSystemManagementRequest request)
        {
            try
            {
                //check account
                var checkAdminAccount = await _unitOfWork.Repository<Admin>().GetAll().FirstOrDefaultAsync(x => x.Id == accountId && x.IsAvailable == true);

                if (checkAdminAccount == null)
                {
                    //account is not avalable to use
                    throw new ErrorResponse(403, (int)AdminAccountErrorEnum.ADMIN_FORBIDDEN,
                                        AdminAccountErrorEnum.ADMIN_FORBIDDEN.GetDisplayName());
                }

                var role = await _unitOfWork.Repository<Role>().FindAsync(x => x.Id == roleId);

                if (role == null)
                {
                    throw new ErrorResponse(404, (int)AdminSystemManagementErrorEnum.SYSTEM_ROLE_NOT_FOUND,
                                                            AdminSystemManagementErrorEnum.SYSTEM_ROLE_NOT_FOUND.GetDisplayName());
                }

                var roleResult = _mapper.Map<UpdateAdminSystemManagementRequest, Role>(request, role);

                roleResult.UpdateAt = Ultils.GetCurrentDatetime();

                await _unitOfWork.Repository<Role>().UpdateDetached(roleResult);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<AdminSystemManagementResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<AdminSystemManagementResponse>(role)
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
