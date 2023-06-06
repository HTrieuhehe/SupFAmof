using AutoMapper;
using SupFAmof.Data.Entity;
using SupFAmof.Service.DTO.Request.Role;
using SupFAmof.Service.DTO.Response;

namespace SupFAmof.API.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            #region Role
            CreateMap<Role, RoleResponse>().ReverseMap();
            CreateMap<CreateRoleRequest, Role>();
            CreateMap<UpdateRoleRequest, Role>();
            #endregion
        }
    }
}
