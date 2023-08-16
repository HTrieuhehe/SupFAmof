using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.Service;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof.API.Controllers
{
    [Route(Helpers.SettingVersionAPI.ApiVersion)]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        /// <summary>
        /// Login Admin
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<StaffResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _staffService.Login(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Invalid External Authentication.");
            }
        }

        /// <summary>
        /// Get all staff for system admin
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<BaseResponsePagingViewModel<StaffResponse>>> GetStaffs
            ([FromQuery] StaffResponse staffResponse, [FromQuery] PagingRequest pagingRequest)
        {
            return await _staffService.GetStaffs(staffResponse, pagingRequest);
        }
    }
}
