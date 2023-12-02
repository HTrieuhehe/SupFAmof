using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupFAmof.Data.Entity;
using SupFAmof.Service.DTO.Request;
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
        [HttpGet("getAccountById")]
        public async Task<ActionResult<BaseResponseViewModel<AccountResponse>>> GetAccountById(int accountId)
        {
            try
            {
                var result = await _accountService.GetAccountById(accountId);
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
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                var result = await _accountService.GetAccountById(account.Id);
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

        /// <summary>
        /// Update Account
        /// </summary>
        /// <returns></returns>
        [HttpPut("update")]
        public async Task<ActionResult<BaseResponseViewModel<AccountResponse>>> UpdateAccount([FromBody] UpdateAccountRequest data)
        {
            try
            {

                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                var result = await _accountService.UpdateAccount(account.Id, data);
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
                var result = await _accountService.Login(data);
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
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                var result = await _accountService.DisableAccount(account.Id);
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

        /// <summary>
        /// Update Account ImgUrl
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPatch("updateAvatar")]
        public async Task<ActionResult<BaseResponseViewModel<AccountResponse>>> UpdateAccountAvatar([FromBody] UpdateAccountAvatar imgUrl)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                var result = await _accountService.UpdateAccountAvatar(account.Id, imgUrl);
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

        /// <summary>
        /// logout 
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPut("logout")]
        public async Task<ActionResult> Logout([FromBody] ExpoTokenLogoutRequest expoPushToken)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                await _accountService.Logout(expoPushToken, account.Id, 1);
                return Ok();
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
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                return await _accountService.EnableAccount(account.Id);
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
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                return await _accountService.InputVerifycationCode(account.Id, code, account.RoleId);
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
        /// Update Account Imformation
        /// </summary>
        /// <returns></returns>
        [HttpPut("updateAccountInforamtion")]
        public async Task<ActionResult<BaseResponseViewModel<AccountInformationResponse>>> UpdateAccountInformation([FromBody] UpdateAccountInformationRequest data)
        {
            try
            {

                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                return await _accountService.UpdateAccountInforamtion(account.Id, data);
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
        /// Update Account Imformation Front IMG
        /// </summary>
        /// <returns></returns>
        [HttpPatch("updateFrontImg")]
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
        [HttpPatch("updateBackImg")]
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
        [HttpPatch("updateFrontAccountInformationCitizen")]
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
                return await _accountService.UpdateCitizenIdentificationFrontImgInformation(account.Id, data);
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
        [HttpPatch("updateBackAccountInformationCitizen")]
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
                return await _accountService.UpdateCitizenIdentificationBackImgInformation(account.Id, data);
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
