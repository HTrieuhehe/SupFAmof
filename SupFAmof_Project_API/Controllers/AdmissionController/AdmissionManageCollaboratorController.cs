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
        private readonly IApplicationService _admissionApplicationService;
        private readonly IPostRegistrationService _postRegistrationService;

        public AdmissionManageCollaboratorController(IAccountService admissionAccountService, IApplicationService admissionApplicationService, IPostRegistrationService postRegistrationService)
        {
            _admissionAccountService = admissionAccountService;
            _admissionApplicationService = admissionApplicationService;
            _postRegistrationService = postRegistrationService;
        }

        [HttpGet("search")]
        public async Task<ActionResult<BaseResponsePagingViewModel<AccountResponse>>> SearchCollabByEmail
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
                if (ex.Error.StatusCode == 404)
                {
                    return NotFound(ex.Error);
                }
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// View number of active collaborator account 
        /// </summary>
        /// <returns></returns>
        [HttpGet("viewNumber")]
        public async Task<ActionResult<BaseResponsePagingViewModel<AccountResponse>>> ViewCollaborator()
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await _admissionAccountService.ViewCollaborator(account.Id);
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

        [HttpPut("reject-request")]
        public async Task<ActionResult<ApplicationResponse>> RejectProblemRequest([FromQuery] int reportId, [FromBody] UpdateAdmissionAccountApplicationRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await _admissionApplicationService.RejectApplication(account.Id,reportId,request);
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

        [HttpPut("approve-request")]
        public async Task<ActionResult<ApplicationResponse>> ApproveProblemRequest([FromQuery] int reportId, [FromBody] UpdateAdmissionAccountApplicationRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await _admissionApplicationService.ApproveApplication(account.Id, reportId, request);
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
        [HttpGet("admission-replied-application")]
        public async Task<ActionResult<BaseResponsePagingViewModel<ApplicationResponse>>> AdmissionGetApplication([FromQuery] AdmissionApplicationResponse filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await _admissionApplicationService.GetAdmissionAccountApplications(account.Id, filter, paging);
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
        [HttpGet("get-all-collab-accounts")]
        public async Task<ActionResult<BaseResponsePagingViewModel<ManageCollabAccountResponse>>> ManageCollaboratorList([FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await _admissionAccountService.GetAllCollabAccount(account.Id,paging);
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


        ///<summary>
        /// Update Collaborator Credential (Premium)
        /// </summary>
        /// 
        [HttpPut("update-collab-credential")]
        public async Task<ActionResult<BaseResponseViewModel<AccountResponse>>> UpdateCollaboratorCredential([FromQuery] int collaboratorAccountId)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _admissionAccountService.UpdateCollaboratorCredential(account.Id, collaboratorAccountId);
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

        ///<summary>
        /// Disable Collaborator Credential (Premium)
        /// </summary>
        /// 
        [HttpPut("disbale-collab-credential")]
        public async Task<ActionResult<BaseResponseViewModel<AccountResponse>>> DisableCollaboratorCredential([FromQuery] int collaboratorAccountId)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _admissionAccountService.DisableCollaboratorCredential(account.Id, collaboratorAccountId);
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

        ///<summary>
        /// View Total Complete Registration (cái ô nho nhỏ trên đầu web admission)
        /// </summary>
        /// 
        [HttpGet("viewCompleteRegistration")]
        public async Task<ActionResult<BaseResponseViewModel<DashboardPostRegistrationResponse>>> DisableCollaboratorCredential()
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _postRegistrationService.GetTotalRegistrationInCurrentMonth(account.Id);
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

        ///<summary>
        /// View Dashboard Analytics
        /// </summary>
        /// 
        [HttpGet("viewAnalytics")]
        public async Task<ActionResult<BaseResponseViewModel<DashboardRegistrationAnalyticsResponse>>> GetAnalytics([FromQuery] DashBoardAnalyticsTimeRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _postRegistrationService.GetAnalyticsInMonth(account.Id, request);
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

        ///<summary>
        /// View Contributor
        /// </summary>
        /// 
        [HttpGet("viewContributor")]
        public async Task<ActionResult<BaseResponsePagingViewModel<DashboardContributionResponse>>> GetContributor([FromQuery] DashBoardContributionTimeRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _postRegistrationService.GetCollaboratorContributionInMonth(account.Id, request);
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
