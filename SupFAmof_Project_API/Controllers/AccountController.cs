using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupFAmof.Data.Entity;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Account;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Service;
using SupFAmof.Service.Service.ServiceInterface;

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
        /// Get Account By Id
        /// </summary>
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
        /// Get Account By Token
        /// </summary>
        /// <returns></returns>
        [HttpGet("/GetAccountByToken/authorization")]
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
        /// Update Account
        /// </summary>
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
        /// Create Account Information
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost("CreateAccountInformation")]
        public async Task<ActionResult<AccountResponse>> CreateAccountInformation([FromBody] CreateAccountInformationRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var accountId = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (accountId == -1)
                {
                    return Unauthorized();
                }
                var result = await _accountService.CreateAccountInformation(accountId, request);
                return Ok(result);
            }
            catch(ErrorResponse ex)
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
