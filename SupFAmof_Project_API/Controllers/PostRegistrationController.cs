using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.Service;
using Microsoft.AspNetCore.Http;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.Enum;

namespace SupFAmof.API.Controllers
{
    [Route(Helpers.SettingVersionAPI.ApiVersion)]
    [ApiController]
    public class PostRegistrationController : ControllerBase
    {
        private readonly IPostRegistrationService _postRegistrationService;

        public PostRegistrationController(IPostRegistrationService postRegistrationService)
        {
            _postRegistrationService = postRegistrationService;
        }

        /// <summary>
        /// This action method handles an HTTP GET request to retrieve a list of PostRegistrationResponse objects associated with a specific accountId.
        /// </summary>
        /// <param name="accountId">The id of the account for which to retrieve the PostRegistrationResponse objects.</param>
        /// <returns>
        /// - 200 OK: Returns a list of PostRegistrationResponse objects associated with the specified accountId.
        /// - 400 Bad Request: If there is an error while processing the request, an ErrorResponse is thrown and returned as a BadRequest.Including there is no Post Registration.
        /// </returns>
        [HttpGet("getById")]
        public async Task<ActionResult<List<PostRegistrationResponse>>> GetPostRegistrationsByAccountId
            ([FromQuery] int accountId, [FromQuery] PagingRequest paging)
        {
            try
            {
                //var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                //var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                //if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Student)
                //{
                //    return Unauthorized();
                //}
                var result = await _postRegistrationService.GetPostRegistrationByAccountId(accountId, paging);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        /// <summary>
        /// This action method handles an HTTP POST request to create a request a PostRegistration objects.
        /// </summary>
        /// <param name="request">The PostRegistrationRequest create the PostRegistration objects.</param>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /create
        ///      {
        ///       "accountId": 5,
        ///       "schoolBusOption": true,
        ///       "postRegistrationDetails": [
        ///         {
        ///           "postId": 1,
        ///            "positionId": 1
        ///         }
        ///       ]
        ///       }
        ///
        /// </remarks>
        /// <response code="200">Returns the newly created item</response>
        /// <response code="400">Failed to create</response>
        [HttpPost("create")]
        public async Task<ActionResult<PostRegistrationResponse>> CreatePostRegistration([FromBody] PostRegistrationRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Student)
                {
                    return Unauthorized();
                }
                var result = await _postRegistrationService.CreatePostRegistration(request);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        /// <summary>
        /// Cancel Post Registration By Id.
        /// </summary>
        /// <param name="postRegistrationId">Id need to be submitted.</param>
        /// <response code="200">Cancel success</response>
        /// <response code="400">Failed to Cancel</response>
        [HttpDelete("cancel")]
        public async Task<ActionResult<BaseResponseViewModel<dynamic>>> CancelPostRegistration([FromQuery] int postRegistrationId)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Student)
                {
                    return Unauthorized();
                }
                return await _postRegistrationService.CancelPostregistration(postRegistrationId);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex);
            }
        }
        /// <summary>
        /// Update Post Registration By Id.
        /// </summary>
        /// <param name="postRegistrationId">Id need to be submitted.</param>
        /// <param name="request">Update object</param>
        ///     {
        ///     "schoolBusOption": true,
        ///     "postRegistrationDetails": [
        ///        {
        ///         "positionId": 0
        ///        }
        ///     ]
        ///     }
        /// <response code="200">Update success</response>
        /// <response code="400">Failed to Update</response>
        [HttpPut("update")]
        public async Task<ActionResult<BaseResponseViewModel<PostRegistrationResponse>>> UpdatePostRegistration
            ([FromQuery] int postRegistrationId, [FromBody] PostRegistrationUpdateRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Student)
                {
                    return Unauthorized();
                }
                var result = await _postRegistrationService.UpdatePostRegistration(postRegistrationId, request);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Approve Request Update for Post Registration
        /// </summary>
        /// <remarks>
        /// true 
        /// </remarks>
        /// <param name="postUpdateHistoryId"></param>
        /// <param name="approveRequest"></param>
        /// <response code="200">Approved success</response>
        /// <response code="400">Failed to Update</response>
        [HttpPut("review-updateRequesst/{postRegistrationId}")]
        public async Task<ActionResult<BaseResponseViewModel<PostRegistrationResponse>>> ApproveRequestUpdate(int postUpdateHistoryId, [FromBody] AproveRequest approveRequest)
        {
            try
            {
                var result =await _postRegistrationService.ApproveUpdateRequest(postUpdateHistoryId, approveRequest.IsApproved);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
    
}
