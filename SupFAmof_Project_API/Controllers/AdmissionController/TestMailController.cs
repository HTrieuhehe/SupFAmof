using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof.API.Controllers.AdmissionController
{
    [Route(Helpers.SettingVersionAPI.ApiAdmisionVersion)]
    [ApiController]
    public class TestMailController : ControllerBase
    {
        private readonly IMailService _mailService;

        public TestMailController(IMailService mailService)
        {
            _mailService = mailService;
        }

        [HttpPost("test-send-email")]
        public async Task<ActionResult> SendEmail(string email)
        {
            try
            {
                await _mailService.SendVerificationEmail(email);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);

            }
        }
    }
}
