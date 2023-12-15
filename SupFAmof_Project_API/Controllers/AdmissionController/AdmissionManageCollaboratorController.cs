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

        public AdmissionManageCollaboratorController(IAccountService admissionAccountService, IApplicationService admissionApplicationService)
        {
            _admissionAccountService = admissionAccountService;
            _admissionApplicationService = admissionApplicationService;
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
                var result = await _admissionAccountService.ViewCollaborator();
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
    }
}
