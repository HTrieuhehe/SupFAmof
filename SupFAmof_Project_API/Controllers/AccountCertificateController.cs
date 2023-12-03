using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.Service;
using SupFAmof.Service.Service;
using Microsoft.AspNetCore.Http;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.DTO.Request.Admission;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Service.ServiceInterface;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof.API.Controllers
{
    [Route(Helpers.SettingVersionAPI.ApiVersion)]
    [ApiController]
    public class AccountCertificateController : ControllerBase
    {
        private readonly IAccountCertificateService _accountCertificateService;
        private readonly ITrainingCertificateService _certificateService;

        public AccountCertificateController(IAccountCertificateService accountCertificateService, ITrainingCertificateService certificateService)
        {
            _accountCertificateService = accountCertificateService;
            _certificateService = certificateService;
        }

        /// <summary>
        /// Get Account Certificate by Token 
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("getbyToken")]
        public async Task<ActionResult<BaseResponsePagingViewModel<AccountCertificateResponse>>> GetAccountCertificateById
        ([FromQuery] AccountCertificateResponse filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                return await _accountCertificateService.GetAccountCertificateByAccountId(account.Id, filter, paging);
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
        [HttpGet("collab-view-registration")]
        public async Task<ActionResult<BaseResponsePagingViewModel<CollabRegistrationsResponse>>> GetRegistrationByCollabId
     ( [FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                return await _certificateService.GetRegistrationByCollabId(account.Id, paging);
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
        [HttpDelete("cancel-registration-collab")]
        public async Task<ActionResult<BaseResponseViewModel<bool>>> CancelCertificateRegistration
       (int certificateRegistrationId)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                await _certificateService.CancelCertificateRegistration(account.Id, certificateRegistrationId);
                return Ok();
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
        [HttpPost("register-certificate-interview")]
        public async Task<ActionResult<BaseResponseViewModel<CollabRegistrationsResponse>>> TrainingCertificateRegistration(int accountId ,[FromBody] TrainingCertificateRegistration request)
        {
            try
            {
                //var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                //var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                //if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                //{
                //    return Unauthorized();
                //}
                var result = await _certificateService.TrainingCertificateRegistration(accountId, request);
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

        /// <summary>
        /// Get Account Certificate by Token 
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("getAllCertificateFromAdmission")]
        public async Task<ActionResult<BaseResponsePagingViewModel<TrainingCertificateResponse>>> GetCertificates
        ([FromQuery] TrainingCertificateResponse filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                return await _certificateService.GetTrainingCertificates(filter, paging);
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
