using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Service;
using SupFAmof.Service.Service.ServiceInterface;
using System.Net.NetworkInformation;
using static SupFAmof.Service.Helpers.Enum;

namespace SupFAmof.API.Controllers
{
    [Route(Helpers.SettingVersionAPI.ApiVersion)]
    [ApiController]
    public class AccountReportProblemController : ControllerBase
    {
        private readonly IAccountReportProblemService _accountReportProblemService;

        public AccountReportProblemController(IAccountReportProblemService accountReportProblemService)
        {
            _accountReportProblemService = accountReportProblemService;
        }

        ///<summary>
        /// Get Account Report Problem by token
        /// </summary>
        /// 
        [HttpGet("getAll")]
        public async Task<ActionResult<BaseResponsePagingViewModel<AccountReportProblemResponse>>> GetAccountReportsByToken
            ([FromQuery] AccountReportProblemResponse filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                return await _accountReportProblemService.GetAccountReportProblemsByToken(account.Id, filter, paging);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        ///<summary>
        /// Create Account Report Problem 
        /// </summary>
        /// 
        [HttpPost("create")]
        public async Task<ActionResult<BaseResponseViewModel<AccountReportProblemResponse>>> CreateAccountReport
            ([FromBody] CreateAccountReportProblemRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                return await _accountReportProblemService.CreateAccountReportProblem(account.Id, request);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
