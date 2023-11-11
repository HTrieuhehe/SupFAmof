using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reso.Core.Custom;
using ServiceStack.DataAnnotations;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Service;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.Enum;

namespace SupFAmof.API.Controllers.AdmissionController
{
    [Route(Helpers.SettingVersionAPI.ApiAdmisionVersion)]
    [ApiController]
    public class AdmissionAttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AdmissionAttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        /// <summary>
        /// Get Attendances History By PositionId
        /// </summary>
        [HttpGet("getByPositionId/{positionId}")]
        public async Task<ActionResult<BaseResponsePagingViewModel<AdmissionAttendanceResponse>>> GetAttendanceHistoryByPositionId
            ([FromQuery] int positionId, [FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _attendanceService.GetAttendanceHistoryByPositionId(account.Id, positionId, paging);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
