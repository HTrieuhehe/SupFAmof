using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.Service;
using Microsoft.AspNetCore.Http;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof.API.Controllers
{
    [Route(Helpers.SettingVersionAPI.ApiVersion)]
    [ApiController]
    public class PostRegistrationController : ControllerBase
    {
        private readonly PostRegistrationIService _postRegistrationService;

        public PostRegistrationController(PostRegistrationIService postRegistrationService)
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
        public async Task<ActionResult<List<PostRegistrationResponse>>> GetPostRegistrationsByAccountId(int accountId)
        {
            try
            {
                var result = await _postRegistrationService.GetPostRegistrationByAccountId(accountId);
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
        public async Task<ActionResult<PostRegistrationResponse>> CreatePostRegistration(PostRegistrationRequest request)
        {
            try
            {
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
        public async Task<ActionResult<BaseResponseViewModel<dynamic>>> CancelPostRegistration(int postRegistrationId)
        {
            try
            {
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
        public async Task<ActionResult<BaseResponseViewModel<PostRegistrationResponse>>> UpdatePostRegistration(int postRegistrationId,PostRegistrationUpdateRequest request)
        {
            try
            {
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
        /// <response code="200">Approved success</response>
        /// <response code="400">Failed to Update</response>
        [HttpPut("review-updateRequesst/{postRegistrationId}")]
        public async Task<ActionResult<BaseResponseViewModel<PostRegistrationResponse>>> ApproveRequestUpdate(int postUpdateHistoryId, [FromBody] AproveRequest approve)
        {
            try
            {
                var result =await _postRegistrationService.ApproveUpdateRequest(postUpdateHistoryId, approve.IsApproved);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
    
}
