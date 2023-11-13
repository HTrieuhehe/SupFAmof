using QRCoder;
using System.Drawing;
using Newtonsoft.Json;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.Service;
using Org.BouncyCastle.Asn1.Ocsp;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof.API.Controllers
{
    [Route(Helpers.SettingVersionAPI.ApiVersion)]
    [ApiController]
    public class CheckAttendanceController : ControllerBase
    {
        private readonly ICheckInService _checkInService;

        public CheckAttendanceController(ICheckInService checkInService)
        {
            _checkInService = checkInService;
        }

        [HttpPost("check-in")]
        public async Task<ActionResult<BaseResponseViewModel<dynamic>>> CheckIn
      ([FromBody] CheckInRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                return await _checkInService.CheckIn(account.Id, request);
            }
            catch (ErrorResponse ex)
            {
                if (ex.Error.StatusCode == 404)
                {
                    return NotFound(ex.Error);
                }
                else if (ex.Error.StatusCode == 401)
                {
                    return Conflict(ex.Error);
                }
                return BadRequest(ex.Error);
            }
        }

        ///<summary>
        ///Check out
        /// </summary>
        /// 
        [HttpPost("check-out")]
        public async Task<ActionResult<BaseResponseViewModel<dynamic>>> CheckOut([FromQuery] int postRegistrationId)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                return await _checkInService.CheckOut(account.Id, postRegistrationId);
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


        [HttpPost("generate-qr")]
        public async Task<ActionResult> QrGenerate([FromBody] QrRequest request)
        {
            try
            {//create qr code 
                var result = await _checkInService.QrGenerate(request);
                return File(result, "image/png");
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
        [HttpGet("attendance-record-collab")]
        public async Task<ActionResult<List<CheckAttendanceResponse>>> AttendanceRecordCollab()
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                var result = await _checkInService.CheckAttendanceHistory(account.Id);
                return Ok(result);
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
        [HttpGet("admission-manage-attendance-record-collab")]
        public async Task<ActionResult<BaseResponsePagingViewModel<CheckAttendancePostResponse>>> AdmissionManageAttendanceRecordCollab([FromQuery] PagingRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await _checkInService.AdmissionManageCheckAttendanceRecord(account.Id, request);
                return Ok(result);
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