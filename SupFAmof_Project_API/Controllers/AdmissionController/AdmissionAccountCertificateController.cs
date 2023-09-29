using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Admission;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Service;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.Enum;

namespace SupFAmof.API.Controllers.AdmissionController
{
    [Route(Helpers.SettingVersionAPI.ApiAdmisionVersion)]
    [ApiController]
    public class AdmissionAccountCertificateController : ControllerBase
    {
        private readonly IAccountCertificateService _accountCertificateService;

        public AdmissionAccountCertificateController(IAccountCertificateService accountCertificateService)
        {
            _accountCertificateService = accountCertificateService;
        }

        /// <summary>
        /// Create Account Certificate 
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost("create")]
        public async Task<ActionResult<BaseResponseViewModel<AccountCertificateResponse>>> CreateAccountCertificate
        ([FromBody] CreateAccountCertificateRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _accountCertificateService.CreateAccountCertificate(account.Id, request);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Update Account Certificate 
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPut("reject-certificate")]
        public async Task<ActionResult<BaseResponseViewModel<AccountCertificateResponse>>> UpdateAccountCertificate
        ([FromQuery] int accountId, [FromQuery] int accountCertificateId)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _accountCertificateService.UpdateAccountCertificate(accountId, accountCertificateId, account.Id);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Get Account Certificates 
        /// </summary>
        /// <returns></returns>
        /// 

        [HttpGet("getAll")]
        public async Task<ActionResult<BaseResponsePagingViewModel<AccountCertificateResponse>>> GetAccountCertificates
            ([FromQuery] AccountCertificateResponse filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _accountCertificateService.GetAccountCertificates(filter, paging);
            }
            catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Get Account Certificate By Id
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("getById")]
        public async Task<ActionResult<BaseResponseViewModel<AccountCertificateResponse>>> GetAccountCertificateById
            ([FromQuery] int accountCertificateId)
        {
            try
            {
                return await _accountCertificateService.GetAccountCertificateById(accountCertificateId);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
