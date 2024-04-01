using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Admission;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Service;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.Enum;

namespace SupFAmof.API.Controllers.AdmissionController
{
    [Route(Helpers.SettingVersionAPI.ApiAdmisionVersion)]
    [ApiController]
    public class AdmissionBanAccountController : ControllerBase
    {
        private readonly IAccountBannedService _accountBannedService;

        public AdmissionBanAccountController(IAccountBannedService accountBannedService)
        {
            _accountBannedService = accountBannedService;
        }


        /// <summary>
        /// Create Account Banned 
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost("create")]
        public async Task<ActionResult<BaseResponseViewModel<AccountBannedResponse>>> CreateAccountBanned
        ([FromBody] CreateAccountBannedRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.TokenExpired)
                {
                    ErrorDetailResponse ex = new ErrorDetailResponse();
                    ex.StatusCode = 401;
                    ex.ErrorCode = 4011;
                    ex.Message = "Login expired";

                    return Unauthorized(ex);
                }
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _accountBannedService.CreateAccountBanned(account.Id, request);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.Error.StatusCode, ex.Error);
            }
        }

        /// <summary>
        /// Update Account Banned 
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPut("update")]
        public async Task<ActionResult<BaseResponseViewModel<AccountBannedResponse>>> UpdateAccountBanned
        ([FromQuery] int accountBannedId, [FromBody] UpdateAccountBannedRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.TokenExpired)
                {
                    ErrorDetailResponse ex = new ErrorDetailResponse();
                    ex.StatusCode = 401;
                    ex.ErrorCode = 4011;
                    ex.Message = "Login expired";

                    return Unauthorized(ex);
                }
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _accountBannedService.UpdateAccountBanned(account.Id, accountBannedId, request);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.Error.StatusCode, ex.Error);
            }
        }

        /// <summary>
        /// Get Account Banned 
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("getAll")]
        public async Task<ActionResult<BaseResponsePagingViewModel<AccountBannedResponse>>> GetAccountBanneds
            ([FromQuery] AccountBannedResponse filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.TokenExpired)
                {
                    ErrorDetailResponse ex = new ErrorDetailResponse();
                    ex.StatusCode = 401;
                    ex.ErrorCode = 4011;
                    ex.Message = "Login expired";

                    return Unauthorized(ex);
                }
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _accountBannedService.GetAccountBanneds(filter, paging);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.Error.StatusCode, ex.Error);
            }
        }


    }
}
