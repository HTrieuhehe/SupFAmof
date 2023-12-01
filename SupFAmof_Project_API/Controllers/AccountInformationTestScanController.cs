using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.DTO.Request.Account;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Service;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.Enum;

namespace SupFAmof.API.Controllers
{
    [Route(Helpers.SettingVersionAPI.ApiVersion)]
    [ApiController]
    public class AccountInformationTestScanController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IApplicationService _applicationService;

        public AccountInformationTestScanController(IAccountService accountService, IApplicationService applicationService)
        {
            _accountService = accountService;
            _applicationService = applicationService;
        }

        /// <summary>
        /// Update Account Imformation Front IMG
        /// </summary>
        /// <returns></returns>
        [HttpPatch("updateAccountInformationCitizenFrontImg")]
        public async Task<ActionResult<BaseResponseViewModel<AccountInformationResponse>>> UpdateCitizenIdentificationFrontImg([FromBody] UpdateCitizenIdentificationFrontImg data)
        {
            try
            {

                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                return await _accountService.UpdateCitizenIdentificationFrontImg(account.Id, data);
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
        /// Update Account Imformation Back IMG
        /// </summary>
        /// <returns></returns>
        [HttpPatch("updateAccountInformationCitizenBacImg")]
        public async Task<ActionResult<BaseResponseViewModel<AccountInformationResponse>>> UpdateCitizenIdentificationBackImg([FromBody] UpdateCitizenIdentificationBackImg data)
        {
            try
            {

                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                return await _accountService.UpdateCitizenIdentificationBackImg(account.Id, data);
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
        /// Update Account Imformation 1
        /// </summary>
        /// <returns></returns>
        [HttpPatch("updateAccountInformationCitizen")]
        public async Task<ActionResult<BaseResponseViewModel<AccountInformationResponse>>> UpdateCitizenIdentification([FromBody] UpdateCitizenIdentification data)
        {
            try
            {

                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                return await _accountService.UpdateCitizenIdentificationInformation(account.Id, data);
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
        /// Update Account Imformation 2
        /// </summary>
        /// <returns></returns>
        [HttpPatch("updateAccountInformationCitizen2")]
        public async Task<ActionResult<BaseResponseViewModel<AccountInformationResponse>>> UpdateCitizenIdentification2([FromBody] UpdateCitizenIdentification2 data)
        {
            try
            {

                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                return await _accountService.UpdateCitizenIdentificationInformation2(account.Id, data);
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

        [HttpDelete("deleteApplication")]
        public async Task<ActionResult<BaseResponseViewModel<bool>>> DeleteApplication([FromQuery] int applicationId)
        {
            try
            {
                return await _applicationService.DeleteApplication(applicationId);
            }
            catch(ErrorResponse ex)
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
