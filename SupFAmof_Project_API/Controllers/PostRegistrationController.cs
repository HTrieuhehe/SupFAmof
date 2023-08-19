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
        private readonly PostRegistrationIService postRegistrationService;

        public PostRegistrationController(PostRegistrationIService postRegistrationService)
        {
            this.postRegistrationService = postRegistrationService;
        }


        /// <summary>
        /// This action method handles an HTTP GET request to retrieve a list of PostRegistrationResponse objects associated with a specific accountId.
        /// </summary>
        /// <param name="accountId">The id of the account for which to retrieve the PostRegistrationResponse objects.</param>
        /// <returns>
        /// - 200 OK: Returns a list of PostRegistrationResponse objects associated with the specified accountId.
        /// - 400 Bad Request: If there is an error while processing the request, an ErrorResponse is thrown and returned as a BadRequest.Including there is no Post Registration.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<List<PostRegistrationResponse>>> GetPostRegistrationsByAccountId(int accountId)
        {
            try
            {
                var result = await postRegistrationService.GetPostRegistrationByAccountId(accountId);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        [HttpPost]
        public async Task<ActionResult<PostRegistrationResponse>> CreatePostRegistration(PostRegistrationRequest request)
        {
            try
            {
                var result = await postRegistrationService.CreatePostRegistration(request);
                return Ok(result);
            }catch(ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpPost("create-post-registration-detail")]
        public async Task<ActionResult<PostRegistrationDetailResponse>> CreatePostRegistrationDetail(PostRegistrationDetailRequest request)
        {
            try
            {
                var result = await postRegistrationService.CreatePostRegistrationDetail(request);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        [HttpDelete("cancel-post-registration")]
        public async Task<ActionResult> CancelPostRegistration(int postRegistrationId)
        {
            try
            {
                postRegistrationService.CancelPostregistration(postRegistrationId);
                StatusViewModel? statusviewModel= new StatusViewModel
                {
                    ErrorCode = 0,
                Success = true,
                Message = "Cancel Successfully"

            };
                return Ok(statusviewModel);
            }catch(ErrorResponse ex)
            {
                return BadRequest(ex);
            }
        }

    }
}
