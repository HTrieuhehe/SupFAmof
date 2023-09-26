using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SupFAmof.Service.Service.ServiceInterface;
using SupFAmof.Service.DTO.Request;

namespace SupFAmof.API.Controllers.AdmissionController
{
    [Route(Helpers.SettingVersionAPI.ApiAdmisionVersion)]
    [ApiController]
    public class TestMailController : ControllerBase
    {
        private readonly ISendMailService _mailService;

        public TestMailController(ISendMailService mailService)
        {
            _mailService = mailService;
        }

        [HttpPost("test-send-email")]
        public async Task<ActionResult> SendEmail
            ([FromBody] MailRequest request)
        {
            try
            {
                await _mailService.SendEmailToUser(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);

            }
        }
    }
}
