using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupFAmof.Data.Entity;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Account;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Service;

namespace SupFAmof.API.Controllers
{
    [Route(Helpers.SettingVersionAPI.ApiVersion)]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        

        /// <summary>
        /// lấy thông tin khách hàng bằng ID
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet("/GetAccountById/accountId")]
        public async Task<ActionResult<AccountResponse>> GetAccountById(int accountId)
        {
            try
            {
                var result = await _accountService.GetAccountById(accountId);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// lấy thông tin khách hàng bằng token
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet("/GetAccountByToken/Authorization")]
        public async Task<ActionResult<AccountResponse>> GetAccountByToken()
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var accountId = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (accountId == -1)
                {
                    return Unauthorized();
                }
                var result = await _accountService.GetAccountById(accountId);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Update thông tin khách hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPut("accountId")]
        public async Task<ActionResult<AccountResponse>> UpdateAccount([FromBody] UpdateAccountRequest data)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var accountId = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (accountId == -1)
                {
                    return Unauthorized();
                }
                var result = await _accountService.UpdateAccount(accountId, data);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Google Login
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult<AccountResponse>> LoginGoogle([FromBody] ExternalAuthRequest data)
        {
            try
            {
                var result = await _accountService.Login(data);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        ///  Logout
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("logout")]
        public async Task<ActionResult> Logout([FromBody] LogoutRequest request)
        {
            await _accountService.Logout(request.FcmToken);
            return Ok();
        }

    }
}
