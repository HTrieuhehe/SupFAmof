using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.Service;
using Microsoft.AspNetCore.Http;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof.API.Controllers
{
    [Route(Helpers.SettingVersionAPI.ApiVersion)]
    [ApiController]
    public class AccountReportController : ControllerBase
    {
        private readonly IAccountReportService _accountReportService;

        public AccountReportController(IAccountReportService accountReportService)
        {
            _accountReportService = accountReportService;
        }

        /// <summary>
        /// Get Account Reports 
        /// </summary>
        /// <returns></returns>
        /// 
        //[HttpGet("getAll")]
        //public async Task<ActionResult<BaseResponsePagingViewModel<AccountReportResponse>>> GetAccountReports
        //([FromQuery] AccountReportResponse filter, [FromQuery] PagingRequest paging)
        //{
        //    try
        //    {
        //        var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        //        var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
        //        if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
        //        {
        //            return Unauthorized();
        //        }
        //        return await _accountReportService.GetAccountReports(filter, paging);
        //    }
        //    catch (ErrorResponse ex)
        //    {
        //        return BadRequest(ex.Error);
        //    }
        //}


        /// <summary>
        /// Get Account Report by Token
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("getByToken")]
        public async Task<ActionResult<BaseResponsePagingViewModel<AccountReportResponse>>> GetAccountReportbyToken
        ([FromQuery] AccountReportResponse filter, [FromQuery] AccountReportFilter dateFilter, [FromQuery] PagingRequest paging, string? searchAccountReport)
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
                return await _accountReportService.GetAccountReportByToken(account.Id, filter, dateFilter, paging, searchAccountReport);
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
        /// Get PostRegistration by Account Report Id
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("getRegistrationByReportId")]
        public async Task<ActionResult<BaseResponseViewModel<ReportPostRegistrationResponse>>> GetPostRegistrationByAccountReportId
        ([FromQuery] int accountReportId)
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
                return await _accountReportService.GetReportRegistrationById(account.Id, accountReportId);
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
