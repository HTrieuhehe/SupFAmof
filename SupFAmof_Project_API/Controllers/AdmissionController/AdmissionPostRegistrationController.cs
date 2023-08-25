using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
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
        /// <param name="postUpdateHistoryId"></param>
        /// <response code="200">Approved success</response>
        /// <response code="400">Failed to Update</response>
        [HttpPut("review-updateRequesst/{postUpdateHistoryId}")]
        public async Task<ActionResult<BaseResponseViewModel<PostRegistrationResponse>>> ApproveRequestUpdate(int postUpdateHistoryId, [FromBody] AproveRequest approve)
        {
            try
            {
                var result = await _postRegistrationService.ApproveUpdateRequest(postUpdateHistoryId, approve.IsApproved);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
