using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.DTO.Request.Account;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.Service;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request.Admission.AccountRequest;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Data.Entity;
using System.Security.Principal;

namespace SupFAmof.API.Controllers.AdmissionController
{
    [Route(Helpers.SettingVersionAPI.ApiAdmisionVersion)]
    [ApiController]
    public class AdmissionAccountController : ControllerBase
    {
        private IAccountService _admissionAccountService;

        public AdmissionAccountController(IAccountService admissionAccountService)
        {
            _admissionAccountService = admissionAccountService;
        }

        /// <summary>
        /// Get Account By Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAccountById")]
        public async Task<ActionResult<AccountResponse>> GetAccountById(int accountId)
        {
            try
            {
                var result = await _admissionAccountService.GetAccountById(accountId);
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
        [HttpGet("getAccountByToken/authorization")]
        public async Task<ActionResult<AccountResponse>> GetAccountByToken()
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await _admissionAccountService.GetAccountById(account.Id);
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
        [HttpPut("update")]
        public async Task<ActionResult<AccountResponse>> UpdateAccount([FromBody] UpdateAdmissionAccountRequest data)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await _admissionAccountService.UpdateAdmissionAccount(account.Id, data);
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
                var result = await _admissionAccountService.Login(data);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Disable Account
        /// </summary>
        /// <returns></returns>
        [HttpPut("disable")]
        public async Task<ActionResult<AccountResponse>> DisableAccount()
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await _admissionAccountService.DisableAccount(account.Id);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
