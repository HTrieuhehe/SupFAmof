using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.Service;
using Microsoft.AspNetCore.Http;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Response;
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

        [HttpGet("get")]
        public async Task<ActionResult<BaseResponseViewModel<CollabInfoReportResponse>>> Get([FromQuery] int accountId)
        {
            try
            {
                var result = await _financialReportService.GetAdmissionFinancialReport(accountId);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpPost("get-account-excel")]
        public async Task<ActionResult> GetAccountExcel([FromQuery] string sheetname)
        {
            try
            {
                 await _financialReportService.GenerateAccountExcel(sheetname);
                return Ok("Complete");
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
