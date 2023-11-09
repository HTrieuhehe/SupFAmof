using SupFAmof.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.Service;
using Microsoft.AspNetCore.Http;
using System.Security.Principal;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Request.Account;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Service.ServiceInterface;
using SupFAmof.Service.DTO.Request.Admission.AccountRequest;

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
        //[HttpGet("getAccountById")]
        //public async Task<ActionResult<AdmissionAccountResponse>> GetAccountAdmissionById(int accountId)
        //{
        //    try
        //    {
        //        var result = await _admissionAccountService.GetAccountAdmissionById(accountId);
        //        return Ok(result);
        //    }
        //    catch (ErrorResponse ex)
        //    {
        //        return BadRequest(ex.Error);
        //    }
        //}

        /// <summary>
        /// Get Account By Token
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAccountByToken/authorization")]
        public async Task<ActionResult<BaseResponseViewModel<AccountResponse>>> GetAccountByToken()
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await _admissionAccountService.GetAccountAdmissionById(account.Id);
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
        public async Task<ActionResult<BaseResponseViewModel<AdmissionAccountResponse>>> UpdateAccount([FromBody] UpdateAdmissionAccountRequest data)
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
        /// Update Account Avatar
        /// </summary>
        /// <returns></returns>
        [HttpPut("updateAvatar")]
        public async Task<ActionResult<BaseResponseViewModel<AdmissionAccountResponse>>> UpdateAccount([FromBody] UpdateAccountAvatar data)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await _admissionAccountService.UpdateAdmissionAccountAvatart(account.Id, data);
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
        public async Task<ActionResult<BaseResponseViewModel<AccountResponse>>> LoginGoogle([FromBody] ExternalAuthRequest data)
        {
            try
            {
                var result = await _admissionAccountService.AdmissionLogin(data);
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
        public async Task<ActionResult<BaseResponseViewModel<AccountResponse>>> DisableAccount()
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

        /// <summary>
        /// logout 
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpDelete("logout")]
        public async Task<ActionResult> Logout([FromQuery] string expoToken)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                await _admissionAccountService.Logout(expoToken, account.Id, 1);
                return Ok();
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Enable Account 
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPut("enable-account")]
        public async Task<ActionResult<BaseResponseViewModel<AccountReactivationResponse>>> EnableProfile()
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _admissionAccountService.EnableAccount(account.Id);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Input Verification
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("input-verify")]
        public async Task<ActionResult<BaseResponseViewModel<dynamic>>> InputVeryfication([FromQuery] int code)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _admissionAccountService.InputVerifycationCode(account.Id, code, account.RoleId);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }


    }
}
