using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Staff;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Service;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof.API.Controllers.AdminController
{
    [Route(Helpers.SettingVersionAPI.ApiAdminVersion)]
    [ApiController]
    public class AdminStaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public AdminStaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        /// <summary>
        /// Get List Staff    
        /// </summary>
        [HttpGet("getAll")]
        public async Task<ActionResult<BaseResponsePagingViewModel<StaffResponse>>> GetStaffs
            ([FromQuery] StaffResponse request, [FromQuery] PagingRequest paging)
        {
            return await _staffService.GetStaffs(request, paging);
        }

        /// <summary>
        /// Get Staff By Id
        /// </summary>
        [HttpGet("getById")]
        public async Task<ActionResult<BaseResponseViewModel<StaffResponse>>> GetStaffById
            ([FromRoute] int staffId)
        {
            return await _staffService.GetStaffById(staffId);
        }

        /// <summary>
        /// Create Admin Account                        
        /// </summary>
        [HttpPost("create")]
        public async Task<ActionResult<BaseResponseViewModel<StaffResponse>>> CreateAdminManager([FromBody] CreateStaffRequest request)
        {
            return await _staffService.CreateAdminManager(request);
        }

        /// <summary>
        /// Update Staff 
        /// </summary>
        [HttpPut("update")]
        public async Task<ActionResult<BaseResponseViewModel<StaffResponse>>> UpdateStaff
            ([FromRoute] int staffId, [FromBody] UpdateStaffRequest request)
        {
            try
            {
                //var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                //var accountId = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                //if (accountId.Id == -1)
                //{
                //    return Unauthorized();
                //}
                var result = await _staffService.UpdateStaff(staffId, request);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
