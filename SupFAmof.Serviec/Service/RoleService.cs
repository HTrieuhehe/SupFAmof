using AutoMapper;
using AutoMapper.QueryableExtensions;
using NetTopologySuite.Algorithm;
using NTQ.Sdk.Core.Filters;
using NTQ.Sdk.Core.Utilities;
using Pipelines.Sockets.Unofficial.Arenas;
using Service.Commons;
using SupFAmof.Data.Entity;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Role;
using SupFAmof.Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SupFAmof.Service.Helpers.ErrorEnum;

namespace SupFAmof.Service.Service
{
    public interface IRoleService
    {
        Task<BaseResponsePagingViewModel<RoleResponse>> GetRoles(RoleResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<RoleResponse>> GetRoleById(int roleId);
        Task<BaseResponseViewModel<RoleResponse>> CreateRole(CreateRoleRequest request);
        Task<BaseResponseViewModel<RoleResponse>> UpdateRole(int role, UpdateRoleRequest request);
    }

    public class RoleService : IRoleService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponseViewModel<RoleResponse>> CreateRole(CreateRoleRequest request)
        {
            try
            {
                var checkRole = _unitOfWork.Repository<Role>()
                                           .Find(x => x.RoleEmail.Contains(request.RoleEmail));

                if (checkRole != null)
                {
                    throw new ErrorResponse(400, (int)RoleErrorEnums.ROLE_INVALID,
                                        RoleErrorEnums.ROLE_INVALID.GetDisplayName());
                }
                var role = _mapper.Map<CreateRoleRequest, Role>(request);
                role.IsActive = true;
                role.CreateAt = DateTime.Now;

                await _unitOfWork.Repository<Role>().InsertAsync(role);
                await _unitOfWork.CommitAsync();

                return new BaseResponseViewModel<RoleResponse>()
                {
                    Status = new StatusViewModel()
                    {
                        Message = "Success",
                        Success = true,
                        ErrorCode = 0
                    },
                    Data = _mapper.Map<RoleResponse>(role)
                };
            }
            catch (ErrorResponse ex)
            {
                throw;
            }
        }

        public Task<BaseResponseViewModel<RoleResponse>> GetRoleById(int roleId)
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponsePagingViewModel<RoleResponse>> GetRoles(RoleResponse filter, PagingRequest paging)
        {
            try
            {
                var role = _unitOfWork.Repository<Role>().GetAll()
                                    .ProjectTo<RoleResponse>(_mapper.ConfigurationProvider)
                                    .DynamicFilter(filter)
                                    .DynamicSort(filter)
                                    .PagingQueryable(paging.Page, paging.PageSize,
                                    Constants.LimitPaging, Constants.DefaultPaging);

                return new BaseResponsePagingViewModel<RoleResponse>()
                {
                    Metadata = new PagingsMetadata()
                    {
                        Page = paging.Page,
                        Size = paging.PageSize,
                        Total = role.Item1
                    },
                    Data = role.Item2.ToList()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<BaseResponseViewModel<RoleResponse>> UpdateRole(int role, UpdateRoleRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
