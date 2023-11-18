using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.Service;
using Microsoft.AspNetCore.Http;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof.API.Controllers.AdmissionController
{
    [Route(Helpers.SettingVersionAPI.ApiAdmisionVersion)]
    [ApiController]
    public class AdmissionFinancialReportController : ControllerBase
    {
        private readonly IFinancialReportService _financialReportService;

        public AdmissionFinancialReportController(IFinancialReportService financialReportService)
        {
            _financialReportService = financialReportService;
        }
        [HttpGet("get-account-report")]
        public async Task<ActionResult<BaseResponsePagingViewModel<CollabReportResponse>>> GetAccountReport([FromQuery] PagingRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await _financialReportService.AccountReportList(request, account.Id);
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
        [HttpPost("get-account-excel")]
        public async Task<ActionResult> GetAccountExcel()
        {

            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await _financialReportService.GenerateAccountExcel(account.Id);
                return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "account_report.xlsx");
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
        [HttpPost("get-account-identity-excel")]
        public async Task<ActionResult> GetAccountIdentityExcel()
        {

            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await _financialReportService.GenerateAccountWithIdentityExcel(account.Id);
                return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "account_report.xlsx");
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
        [HttpPost("get-od-monthly-excel")]
        public async Task<ActionResult> GenerateOpendayReportMonthly(int accountId,[FromQuery] FinancialReportRequest request)
        {

            try
            {
                //var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                //var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                //if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                //{
                //    return Unauthorized();
                //}
                var result = await _financialReportService.GenerateOpendayReportMonthly(accountId,request);
                return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "account_report.xlsx");
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
