using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.DTO.Request.Admission;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Service;
using SupFAmof.Service.Service.ServiceInterface;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Response;
using static SupFAmof.Service.Helpers.Enum;

namespace SupFAmof.API.Controllers.AdmissionController
{
    [Route(Helpers.SettingVersionAPI.ApiAdmisionVersion)]
    [ApiController]
    public class AdmissionTrainingCertificateController : ControllerBase
    {
        private readonly ITrainingCertificateService _certificateService;

        public AdmissionTrainingCertificateController(ITrainingCertificateService certificateService)
        {
            _certificateService = certificateService;
        }

        /// <summary>
        /// Get List Training Certificates
        /// </summary>    
        [HttpGet("getAll")]
        public async Task<ActionResult<BaseResponsePagingViewModel<TrainingCertificateResponse>>> GetPostTitles
            ([FromQuery] TrainingCertificateResponse filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                return await _certificateService.GetTrainingCertificates(filter, paging);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Get Training Certificates By Id                        
        /// </summary>
        [HttpGet("getById")]
        public async Task<ActionResult<BaseResponseViewModel<TrainingCertificateResponse>>> GetTrainingCertificateById
            ([FromQuery] int trainingCertificateId)
        {
            try
            {
                return await _certificateService.GetTrainingCertificateById(trainingCertificateId);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Create Post                       
        /// </summary>
        [HttpPost("create")]
        public async Task<ActionResult<BaseResponseViewModel<TrainingCertificateResponse>>> CreateTrainingCertificate
            ([FromBody] CreateTrainingCertificateRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _certificateService.CreateTrainingCertificate(request);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Update Post                        
        /// </summary>
        [HttpPut("update")]
        public async Task<ActionResult<BaseResponseViewModel<TrainingCertificateResponse>>> UpdateTrainingCertificate
            ([FromQuery] int trainingCertificateId, [FromBody] UpdateTrainingCertificateRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _certificateService.UpdateTrainingCertificate(trainingCertificateId, request);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
