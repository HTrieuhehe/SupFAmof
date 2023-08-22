using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Service;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.Enum;

namespace SupFAmof.API.Controllers
{
    [Route(Helpers.SettingVersionAPI.ApiVersion)]
    [ApiController]
    public class AccountCertificateController : ControllerBase
    {
        private readonly IAccountCertificateService _accountCertificateService;

        public AccountCertificateController(IAccountCertificateService accountCertificateService)
        {
            _accountCertificateService = accountCertificateService;
        }

        /// <summary>
        /// Get Account Certificate by Token 
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("getbyToken")]
        public async Task<ActionResult<BaseResponsePagingViewModel<AccountCertificateResponse>>> GetAccountCertificateById
        ([FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _accountCertificateService.GetAccountCertificateByAccountId(account.Id, paging);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

    }
}
