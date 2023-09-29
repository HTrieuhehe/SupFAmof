using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.Service.ServiceInterface;

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
    }
}
