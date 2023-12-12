using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.Service;
using Microsoft.AspNetCore.Http;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof.API.Controllers.AdmissionController
{
    [Route(Helpers.SettingVersionAPI.ApiAdmisionVersion)]
    [ApiController]
    public class AdmissionPostRegistrationController : ControllerBase
    {
        private readonly IPostRegistrationService _postRegistrationService;

        public AdmissionPostRegistrationController(IPostRegistrationService postRegistrationService)
        {
            _postRegistrationService = postRegistrationService;
        }
        /// <summary>
        /// Approve Request Update for Post Registration
        /// </summary>
        /// <remarks>
        /// true 
        /// </remarks>
        /// <param name="list">Assign list Id to update</param>
        /// <param name="approve"></param>
        /// <response code="200">Approved success</response>
        /// <response code="400">Failed to Update</response>
        [HttpPut("review-updateRequest/")]
        public async Task<ActionResult<BaseResponseViewModel<PostRegistrationResponse>>> ApproveRequestUpdate([FromBody] AproveUpdateIdRequest list, [FromQuery] AproveRequest approve)
        {
            try
            {
                var result = await _postRegistrationService.ApproveUpdateRequest(list.Ids, approve.IsApproved);
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
        /// Get all post registration by post created by admssionId
        /// </summary>
        /// <remarks>
        /// true 
        /// </remarks>
        /// <response code="200">Get success</response>
        /// <response code="400">Failed to get</response>
        [HttpGet("get-postRegistrationbyAdmissionAccountId")]
        public async Task<ActionResult<BaseResponseViewModel<List<AdmissionPostsResponse>>>> AdmssionPostRegistrations([FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }

                var result = await _postRegistrationService.AdmssionPostRegistrations(account.Id, paging);
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
        /// Get all post registration by post created by admssionId
        /// </summary>
        /// <remarks>
        /// true 
        /// </remarks>
        /// <response code="200">Get success</response>
        /// <response code="400">Failed to get</response>
        [HttpGet("get-postRegistrationUpdateRequest-by-Admission-AccountId")]
        public async Task<ActionResult<BaseResponseViewModel<List<AdmissionPostsResponse>>>> AdmissionUpdateRequests([FromQuery] PagingRequest paging, [FromQuery] FilterUpdateRequestResponse IdFilter)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }

                var result = await _postRegistrationService.AdmissionUpdateRequests(account.Id, paging,IdFilter);
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
        /// Approve Join Request Post
        /// </summary>
        /// <remarks>
        /// true 
        /// </remarks>
        /// <param name="ids"></param>
        /// <param name="approve"></param>
        /// <response code="200">Approved success</response>
        /// <response code="400">Failed to Update</response>
        [HttpPut("review-joinRequest/")]
        public async Task<ActionResult<BaseResponseViewModel<PostRegistrationResponse>>> ApprovePostRegistrationRequest([FromBody] List<int> ids, [FromQuery] AproveRequest approve)
        {
            try
            {

                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }

                var result = await _postRegistrationService.ApprovePostRegistrationRequest(account.Id,ids, approve.IsApproved);
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
        /// Get Account Register in Post Position
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("getAccountByPostPositionId")]
        public async Task<ActionResult<BaseResponsePagingViewModel<PostRegistrationResponse>>> GetAccountByPostPositionId
            ([FromQuery] int positionId, string? searchEmail, [FromQuery] PostRegistrationResponse filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _postRegistrationService.GetAccountByPostPositionId(account.Id, positionId, searchEmail, filter,  paging);
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

        [HttpGet("get-work-list-position")]
        public async Task<ActionResult<BaseResponsePagingViewModel<PostRegistrationResponse>>> GetRegistrationByPositionIdAdmission
        ([FromQuery] int? positionId ,[FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _postRegistrationService.GetRegistrationByPositionIdAdmission(account.Id, paging,positionId);
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
        [HttpDelete("cancel-post-registration")]
        public async Task<ActionResult<BaseResponseViewModel<List<PostRegistrationResponse>>>> CancelPostRegistrationAdmission
         ([FromBody] List<int> PostRegistrationIds)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await _postRegistrationService.CancelPostRegistrationAdmission(PostRegistrationIds, account.Id);
                return result;
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
