using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.DTO.Request.AccounBanking;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Service;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.Enum;

namespace SupFAmof.API.Controllers.AdminController
{
    [Route(Helpers.SettingVersionAPI.ApiAdminVersion)]
    [ApiController]
    public class AdministrationManageAdmissionController : ControllerBase
    {
        private readonly IAdmissionCredentialService _admissionCredentialService;

        public AdministrationManageAdmissionController(IAdmissionCredentialService admissionCredentialService)
        {
            _admissionCredentialService = admissionCredentialService;
        }

        ///<summary>
        /// Upgrade Admission Credential
        /// </summary>
        /// 
        [HttpPost("upgradeCredential")]
        public async Task<ActionResult<BaseResponseViewModel<AccountBankingResponse>>> CreateAdmissionCredential([FromBody] int admissionAccountId)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.SystemAdmin)
                {
                    return Unauthorized();
                }
                var result = await _admissionCredentialService.CreateAdmissionCredential(account.Id, admissionAccountId);
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

        ///<summary>
        /// Upgrade Admission Credential
        /// </summary>
        /// 
        [HttpPost("disableCredential")]
        public async Task<ActionResult<BaseResponseViewModel<AccountResponse>>> DisableAdmissionCredential([FromBody] int admissionAccountId)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.SystemAdmin)
                {
                    return Unauthorized();
                }
                var result = await _admissionCredentialService.DisableAdmissionCredential(account.Id, admissionAccountId);
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
