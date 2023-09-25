using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof.API.Controllers
{
    [Route(Helpers.SettingVersionAPI.ApiVersion)]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IMailService _mailService;

        public MailController(IMailService mailService)
        {
            _mailService = mailService;
        }

        [HttpPost]
        public async Task<ActionResult> SendEmail(string email)
        {
            try
            {
                await _mailService.SendVerificationEmail(email);
                return Ok();
            }catch(Exception ex)
            {
                return BadRequest(ex);

            }
        }
    }
}
