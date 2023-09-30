using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.Service.ServiceInterface;
using AutoMapper.Internal;

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

        [HttpPost("test-send-email-verification")]
        public async Task<ActionResult> SendEmail
            ([FromBody] MailVerificationRequest request)
        {
            try
            {
                await _mailService.SendEmailVerification(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);

            }
        }

        [HttpPost("test-send-email-booking")]
        public async Task<ActionResult> SendEmail
                  ([FromBody] MailBookingRequest request)
        {
            try
            {
                await _mailService.SendEmailBooking(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);

            }
        }
    }
}
