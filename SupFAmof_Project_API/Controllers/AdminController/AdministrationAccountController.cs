using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupFAmof.Data.Entity;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Staff;
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
    public class AdministrationAccountController : ControllerBase
    {
        private readonly IAdminAccountService _adminAccountService;

        public AdministrationAccountController(IAdminAccountService adminAccountService)
        {
            _adminAccountService = adminAccountService;
        }

        /// <summary>
        /// Administration Login 
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<BaseResponseViewModel<dynamic>>> Login
            ([FromBody] LoginRequest request)
        {
            try
            {
                return await _adminAccountService.Login(request);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Get List Administrators    
        /// </summary>
        [HttpGet("getAll")]
        public async Task<ActionResult<BaseResponsePagingViewModel<AdminAccountResponse>>> GetAdmins
            ([FromQuery] AdminAccountResponse request, [FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.SystemAdmin)
                {
                    return Unauthorized();
                }
                return await _adminAccountService.GetAdmins(request, paging);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Get Administrators By Id
        /// </summary>
        [HttpGet("getById")]
        public async Task<ActionResult<BaseResponseViewModel<AdminAccountResponse>>> GetAdminById()
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.SystemAdmin)
                {
                    return Unauthorized();
                }
                return await _adminAccountService.GetAdminById(account.Id);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Create Admin Account                        
        /// </summary>
        [HttpPost("create")]
        public async Task<ActionResult<BaseResponseViewModel<AdminAccountResponse>>> CreateAdminManager([FromBody] CreateAdminAccountRequest request)
        {
            return await _adminAccountService.CreateAdminManager(request);
        }

        /// <summary>
        /// Update Administrators 
        /// </summary>
        [HttpPut("update")]
        public async Task<ActionResult<BaseResponseViewModel<AdminAccountResponse>>> UpdateAdmin
            ([FromBody] UpdateAdminAccountRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.SystemAdmin)
                {
                    return Unauthorized();
                }
                return await _adminAccountService.UpdateAdmin(account.Id, request);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
