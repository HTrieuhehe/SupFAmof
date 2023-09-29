using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.Service;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.Exceptions;

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
        public async Task<ActionResult> CheckIn
      ([FromBody] CheckInRequest request)
        {
            try
            {
                await _checkInService.CheckIn(request);
                return Ok("Check In completed");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);

            }
        }

        ///<summary>
        ///Check out
        /// </summary>
        /// 
        [HttpPost("check-out")]
        public async Task<ActionResult<BaseResponseViewModel<dynamic>>> CheckOut([FromQuery] int accountId, [FromBody] CheckOutRequest request)
        {
            try
            {
                //var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                //var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                //if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                //{
                //    return Unauthorized();
                //}
                return await _checkInService.CheckOut(accountId, request);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
