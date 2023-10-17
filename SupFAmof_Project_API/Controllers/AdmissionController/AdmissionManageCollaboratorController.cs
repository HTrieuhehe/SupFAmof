using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.Service;
using NTQ.Sdk.Core.CustomModel;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.Ocsp;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof.API.Controllers.AdmissionController
{
    [Route(Helpers.SettingVersionAPI.ApiAdminVersion)]
    [ApiController]
    public class AdmissionManageCollaboratorController : ControllerBase
    {
        private readonly IAccountService _admissionAccountService;
        private readonly IAccountReportProblemService _admissionAccountReportProblemService;

        public AdmissionManageCollaboratorController(IAccountService admissionAccountService, IAccountReportProblemService admissionAccountReportProblemService)
        {
            _admissionAccountService = admissionAccountService;
            _admissionAccountReportProblemService = admissionAccountReportProblemService;
        }

        [HttpGet]
        public async Task<ActionResult<Service.DTO.Response.BaseResponsePagingViewModel<AccountResponse>>> SearchCollabByEmail
            ([FromQuery] string email, [FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await _admissionAccountService.SearchCollaboratorByEmail(email, paging);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        [HttpPut("reject-request")]
        public async Task<ActionResult<AccountReportProblemResponse>> RejectProblemRequest(int reportId, UpdateAdmissionAccountReportProblemRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await _admissionAccountReportProblemService.RejectReportProblem(account.Id,reportId,request);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        [HttpPut("approve-request")]
        public async Task<ActionResult<AccountReportProblemResponse>> ApproveProblemRequest(int reportId, UpdateAdmissionAccountReportProblemRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await _admissionAccountReportProblemService.ApproveReportProblem(account.Id, reportId, request);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("admission-replied-application")]
        public async Task<ActionResult<Service.DTO.Response.BaseResponsePagingViewModel<AccountReportProblemResponse>>> AdmissionReplyApplication([FromQuery] AdmissionAccountReportProblemResponse filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await _admissionAccountReportProblemService.GetAdmissionAccountReportProblems(account.Id, filter, paging);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }

        }

    }
}
