using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.Service;
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
    [Route(Helpers.SettingVersionAPI.ApiAdmisionVersion)]
    [ApiController]
    public class AdmissionManageCollaboratorController : ControllerBase
    {
        private readonly IAccountService _admissionAccountService;
        private readonly IComplaintService _admissionComplaintService;

        public AdmissionManageCollaboratorController(IAccountService admissionAccountService, IComplaintService admissionComplaintService)
        {
            _admissionAccountService = admissionAccountService;
            _admissionComplaintService = admissionComplaintService;
        }

        [HttpGet("search")]
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

        /// <summary>
        /// View number of active collaborator account 
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<ActionResult<Service.DTO.Response.BaseResponsePagingViewModel<AccountResponse>>> ViewCollaborator()
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await _admissionAccountService.ViewCollaborator();
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        [HttpPut("reject-request")]
        public async Task<ActionResult<CompaintResponse>> RejectProblemRequest(int reportId, UpdateAdmissionAccountReportProblemRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await _admissionComplaintService.RejectReportProblem(account.Id,reportId,request);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        [HttpPut("approve-request")]
        public async Task<ActionResult<CompaintResponse>> ApproveProblemRequest(int reportId, UpdateAdmissionAccountReportProblemRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await _admissionComplaintService.ApproveReportProblem(account.Id, reportId, request);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("admission-replied-application")]
        public async Task<ActionResult<BaseResponsePagingViewModel<CompaintResponse>>> AdmissionReplyApplication([FromQuery] AdmissionComplaintResponse filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await _admissionComplaintService.GetAdmissionAccountReportProblems(account.Id, filter, paging);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }

        }

    }
}
