using SupFAmof.Data.Entity;
using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.Service;
using Microsoft.AspNetCore.Http;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.Service.ServiceInterface;

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
        /// <param name="paging"></param>
        /// <param name="filter"></param>
        /// <param name="accountId">The id of the account for which to retrieve the PostRegistrationResponse objects.</param>
        /// <returns>
        /// - 200 OK: Returns a list of PostRegistrationResponse objects associated with the specified accountId.
        /// - 400 Bad Request: If there is an error while processing the request, an ErrorResponse is thrown and returned as a BadRequest.Including there is no Post Registration.
        /// </returns>
        [HttpGet("getById")]
        public async Task<ActionResult<CollabRegistrationUpdateViewResponse>> GetPostRegistrationsByAccountId
          ([FromQuery] PagingRequest paging, [FromQuery] CollabRegistrationUpdateViewResponse filter, [FromQuery] FilterPostRegistrationResponse statusFilter)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                var result = await _postRegistrationService.GetPostRegistrationByAccountId(account.Id, paging,filter, statusFilter);
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
        public async Task<ActionResult<CollabRegistrationResponse>> CreatePostRegistration(PostRegistrationRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                var result = await _postRegistrationService.CreatePostRegistration(account.Id, request);
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
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                return await _postRegistrationService.CancelPostregistration(account.Id, postRegistrationId);
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
        [HttpPost("update")]
        public async Task<ActionResult<BaseResponseViewModel<CollabRegistrationResponse>>> UpdatePostRegistration(PostRegistrationUpdateRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                var result = await _postRegistrationService.UpdatePostRegistration(account.Id, request);
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
        /// This action method handles an HTTP GET request to retrieve a list of PostRgUpdateRequestResponse objects associated with a specific accountId.
        /// </summary>
        /// <param name="accountId">The id of the account for which to retrieve the PostRegistrationResponse objects.</param>
        /// <param name="filter">The id of the account for which to retrieve the PostRegistrationResponse objects.</param>
        /// <returns>
        /// - 200 OK: Returns a list of PostRgUpdateRequestResponse objects associated with the specified accountId.
        /// - 400 Bad Request: If there is an error while processing the request, an ErrorResponse is thrown and returned as a BadRequest.Including there is no Post Registration.
        /// </returns>
        [HttpGet("get-update-request")]
        public async Task<ActionResult<BaseResponsePagingViewModel<PostRgupdateHistoryResponse>>> GetUpdateRequestByAccountId
        ([FromQuery] PostRgupdateHistoryResponse filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                var result = await _postRegistrationService.GetUpdateRequestByAccountId(account.Id, filter, paging);
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

        #region Code của Hải Triều

        /// <summary>
        /// Get Post Registration near current time to check in.
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpGet("getCheckInPostRegistration")]
        public async Task<ActionResult<BaseResponsePagingViewModel<CollabRegistrationResponse>>> GetPostRegistrationCheckIn([FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                return await _postRegistrationService.GetPostRegistrationCheckIn(account.Id, paging);
            }
            catch(ErrorResponse ex) 
            {
                if (ex.Error.StatusCode == 404)
                {
                    return NotFound(ex.Error);
                }
                return BadRequest(ex.Error);
            } 
        }

        #endregion

    //    [HttpGet("Filter-status")]
    //    public async Task<ActionResult<BaseResponsePagingViewModel<CollabRegistrationResponse>>> FilterPostRegistration
    //([FromQuery] PagingRequest paging, [FromQuery] FilterPostRegistrationResponse filter)
    //    {
    //        try
    //        {
    //            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
    //            var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
    //            if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
    //            {
    //                return Unauthorized();
    //            }
    //            var result = await _postRegistrationService.FilterPostRegistration(account.Id, filter, paging);
    //            return Ok(result);
    //        }
    //        catch (ErrorResponse ex)
    //        {
    //            if (ex.Error.StatusCode == 404)
    //            {
    //                return NotFound(ex.Error);
    //            }
    //            return BadRequest(ex.Error);
    //        }
    //    }
        [HttpPut("update-SchoolBus")]
        public async Task<ActionResult<BaseResponseViewModel<dynamic>>> UpdateSchoolBusOption
 (UpdateSchoolBusRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                var result = await _postRegistrationService.UpdateSchoolBus(account.Id, request);
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
    }
}

