using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.Service;
using Microsoft.AspNetCore.Http;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using FirebaseAdmin.Auth.Multitenancy;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.DTO.Request.Admission;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Service.ServiceInterface;

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
        public async Task<ActionResult<BaseResponsePagingViewModel<TrainingCertificateResponse>>> GetTratiningCertificates
            ([FromQuery] TrainingCertificateResponse filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _certificateService.GetTrainingCertificates(filter, paging);
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
        /// Get Training Certificates By Id                        
        /// </summary>
        [HttpGet("getById")]
        public async Task<ActionResult<BaseResponseViewModel<TrainingCertificateResponse>>> GetTrainingCertificateById
            ([FromQuery] int trainingCertificateId)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _certificateService.GetTrainingCertificateById(trainingCertificateId);
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
                return await _certificateService.CreateTrainingCertificate(account.Id, request);
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
                return await _certificateService.UpdateTrainingCertificate(account.Id, trainingCertificateId, request);
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
        /// Search Training Ceritificate by Name or Type
        /// </summary>
        /// 
        [HttpGet("search")]
        public async Task<ActionResult<BaseResponsePagingViewModel<TrainingCertificateResponse>>> SearchTrainingCertificate
            ([FromQuery] string search, [FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _certificateService.SearchTrainingCertificate(search, paging);
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
        /// Disable/Delete Training Ceritificate 
        /// </summary>
        /// 
        [HttpDelete("disable")]
        public async Task<ActionResult<BaseResponseViewModel<bool>>> DisableTrainingCertificate
            ([FromQuery] int trainingCertificateId)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _certificateService.DisableTrainingCertificate(account.Id, trainingCertificateId);
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


        [HttpPost("create-class-interview")]
        public async Task<ActionResult<BaseResponseViewModel<dynamic>>> CreateDaysForCertificateInterview([FromBody] EventDaysCertificate request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await _certificateService.CreateDaysForCertificateInterview(account.Id, request);
                return Ok(result);
            }
            catch(ErrorResponse ex)
            {
                throw;
            }
        }
        [HttpPost("register-certificate-interview")]
        public async Task<ActionResult> TrainingCertificateRegistration( [FromBody] TrainingCertificateRegistration request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                var result = await _certificateService.TrainingCertificateRegistration(account.Id, request);
                return Ok(result); 
            }
            catch (ErrorResponse ex)
            {
                throw;
            }
        }
        [HttpPost("assign-eventDay-account")]
        public async Task<ActionResult> AssignDayToRegistration( [FromBody] List<AssignEventDayToAccount> requests)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await _certificateService.AssignDayToRegistration(account.Id, requests);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                throw;
            }
        }

        [HttpGet("view-collaborator-class")]
        public async Task<ActionResult<BaseResponsePagingViewModel<ViewCollabInterviewClassResponse>>> GetCollabInClass
          ([FromQuery]int eventDayId,[FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _certificateService.GetCollabInClass(account.Id,eventDayId, paging);
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
        [HttpPut("update-event-day")]
        public async Task<ActionResult<BaseResponseViewModel<dynamic>>> UpdateDaysForCertificateInterview
        ([FromQuery] int eventDayId, [FromBody] UpdateDaysCertifcate request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _certificateService.UpdateDaysForCertificateInterview(account.Id, eventDayId, request);
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
