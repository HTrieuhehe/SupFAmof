using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Admin;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admin;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Service;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.Enum;

namespace SupFAmof.API.Controllers.AdminController
{
    [Route(Helpers.SettingVersionAPI.ApiAdminVersion)]
    [ApiController]
    public class AdministrationSystemManagementController : ControllerBase
    {
        #region define in use variables

        private readonly ISystemManagementService _systemManagementService;

        public AdministrationSystemManagementController(ISystemManagementService systemManagementService)
        {
            _systemManagementService = systemManagementService;
        }

        #endregion

        /// <summary>
        /// Create Role
        /// </summary>
        [HttpPost("create")]
        public async Task<ActionResult<BaseResponseViewModel<AdminSystemManagementResponse>>> CreateSystemRole([FromBody] CreateAdminSystemManagementRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.SystemAdmin)
                {
                    return Unauthorized();
                }
                return await _systemManagementService.CreateRole(account.Id, request);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.Error.StatusCode, ex.Error);
            }
        }

        /// <summary>
        /// Create Role
        /// </summary>
        [HttpPut("update")]
        public async Task<ActionResult<BaseResponseViewModel<AdminSystemManagementResponse>>> UpdateSystemRole([FromQuery] int roleId, [FromBody] UpdateAdminSystemManagementRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.SystemAdmin)
                {
                    return Unauthorized();
                }
                return await _systemManagementService.UpdateRole(account.Id, roleId, request);
            }
            catch (ErrorResponse ex)
            {
                if (ex.Error.StatusCode == 404)
                {
                    return NotFound(ex.Error);
                }

                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Create Role
        /// </summary>
        [HttpGet("getAll")]
        public async Task<ActionResult<BaseResponsePagingViewModel<AdminSystemManagementResponse>>> GetSystemRoles([FromQuery] AdminSystemManagementResponse filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.SystemAdmin)
                {
                    return Unauthorized();
                }
                return await _systemManagementService.GetRoles(account.Id, filter, paging);
            }
            catch (ErrorResponse ex)
            {
                if (ex.Error.StatusCode == 404)
                {
                    return NotFound(ex.Error);
                }

                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Create Role
        /// </summary>
        [HttpDelete("disable")]
        public async Task<ActionResult<BaseResponseViewModel<AdminSystemManagementResponse>>> DisableSystemRole([FromQuery] int roleId)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.SystemAdmin)
                {
                    return Unauthorized();
                }
                return await _systemManagementService.DisableRole(account.Id, roleId);
            }
            catch (ErrorResponse ex)
            {
                if (ex.Error.StatusCode == 404)
                {
                    return NotFound(ex.Error);
                }

                return BadRequest(ex.Error);
            }
        }
    }
}
