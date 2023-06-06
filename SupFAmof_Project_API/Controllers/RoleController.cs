using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Role;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.Service;
using System.Net.NetworkInformation;

namespace SupFAmof.API.Controllers
{
    [Route(Helpers.SettingVersionAPI.ApiVersion)]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Get List Areas    
        /// </summary>    
        [HttpGet]
        public async Task<ActionResult<BaseResponsePagingViewModel<RoleResponse>>> GetRoles
            ([FromQuery] RoleResponse request, [FromQuery] PagingRequest paging)
        {
            try
            {
                return await _roleService.GetRoles(request, paging);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Create                        
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<BaseResponseViewModel<RoleResponse>>> CreateRole([FromBody] CreateRoleRequest request)
        {
            try
            {
                return await _roleService.CreateRole(request);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
