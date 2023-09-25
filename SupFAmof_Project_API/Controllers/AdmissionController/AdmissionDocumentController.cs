using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.Service;
using Microsoft.AspNetCore.Http;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof.API.Controllers.AdmissionController
{
    [Route(Helpers.SettingVersionAPI.ApiAdmisionVersion)]
    [ApiController]
    public class AdmissionDocumentController : ControllerBase
    {
        private readonly IDocumentService documentService;

        public AdmissionDocumentController(IDocumentService documentService)
        {
            this.documentService = documentService;
        }
        /// <summary>
        /// Get All Document
        /// </summary>
        /// <remarks>
        /// true 
        /// </remarks>
        /// <response code="200">Get All</response>
        /// <response code="400">Failed to retrieve any information</response>
        [HttpGet]
        public async Task<ActionResult<List<AdmissionDocumentResponse>>> GetAllDocument([FromQuery] PagingRequest paging) {

            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await documentService.GetDocuments(paging);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }

        }

        /// <summary>
        /// Create Document
        /// </summary>
        /// <remarks>
        /// true 
        /// </remarks>
        /// <response code="200">Create Success</response>
        /// <response code="400">Failed to create document</response>
        [HttpPost]
        public async Task<ActionResult> CreateDocument(DocumentRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await documentService.CreateDocument(request);
                return Ok(result);
            }catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }

        }
        /// <summary>
        /// Update Document
        /// </summary>
        /// <remarks>
        /// true 
        /// </remarks>
        /// <response code="200">Update Success</response>
        /// <response code="400">Failed to update any information</response>
        [HttpPut("update-document/{documentId}")]
        public async Task<ActionResult> UpdateDocument(int documentId , DocumentUpdateRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await documentService.UpdateDocument(documentId,request);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }

        }

        [HttpDelete]
        public async Task<ActionResult<AdmissionDocumentResponse>> DisableDocument(int documentId)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                var result = await documentService.DisableDocument(documentId);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
