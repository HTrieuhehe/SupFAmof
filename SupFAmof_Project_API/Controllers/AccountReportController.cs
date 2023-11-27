﻿using Microsoft.AspNetCore.Http;
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
        ([FromQuery] AccountReportResponse filter, [FromQuery] AccountReportFilter dateFilter, [FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                return await _accountReportService.GetAccountReportByToken(account.Id, filter, dateFilter, paging);
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
