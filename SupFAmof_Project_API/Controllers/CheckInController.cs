using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.Service.ServiceInterface;
using NTQ.Sdk.Core.Filters;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.Service;
using SupFAmof.Service.DTO.Response;

namespace SupFAmof.API.Controllers
{
    [Route(Helpers.SettingVersionAPI.ApiVersion)]
    [ApiController]
    public class CheckInController : ControllerBase
    {
        private readonly ICheckInService _checkInService;

        public CheckInController(ICheckInService checkInService)
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
        public async Task<ActionResult<BaseResponseViewModel<dynamic>>> CheckOut([FromBody] CheckOutRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                return await _checkInService.CheckOut(account.Id, request);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
