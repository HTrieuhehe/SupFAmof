using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.Service;
using Microsoft.AspNetCore.Http;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof.API.Controllers
{
    [Route(Helpers.SettingVersionAPI.ApiVersion)]
    [ApiController]
    public class AccountBannedController : ControllerBase
    {
        private readonly IAccountBannedService _accountBannedService;

        public AccountBannedController(IAccountBannedService accountBannedService)
        {
            _accountBannedService = accountBannedService;
        }

        /// <summary>
        /// Get Account Banned 
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("getByToken")]
        public async Task<ActionResult<BaseResponsePagingViewModel<AccountBannedResponse>>> GetAccountBannedByToken
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
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                return await _accountBannedService.GetAccountBannedByToken(account.Id, filter, paging);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.Error.StatusCode, ex.Error);
            }
        }

        /// <summary>
        /// Get Current Account Banned 
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("getCurrentAccountBanned")]
        public async Task<ActionResult<BaseResponseViewModel<AccountBannedResponse>>> GetCurrentAccountBanned()
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
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                return await _accountBannedService.GetCuurentAccountBanned(account.Id);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.Error.StatusCode, ex.Error);
            }
        }
    }
}
