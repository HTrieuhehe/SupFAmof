using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Request.Admission;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Response.Admission;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Service;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.Enum;

namespace SupFAmof.API.Controllers.AdmissionController
{
    [Route(Helpers.SettingVersionAPI.ApiAdmisionVersion)]
    [ApiController]
    public class AdmissionContractController : ControllerBase
    {
        private readonly IContractService _contractService;

        public AdmissionContractController(IContractService contractService)
        {
            _contractService = contractService;
        }

        /// <summary>
        /// Get Contracts
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("getAll")]
        public async Task<ActionResult<BaseResponsePagingViewModel<AdmissionApplicationResponse>>> GetAdmissionContracts
            ([FromQuery] AdmissionApplicationResponse filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }

                return await _contractService.GetAdmissionContracts(account.Id, filter, paging);
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
        /// Get Contract By id
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("getById")]
        public async Task<ActionResult<BaseResponseViewModel<AdmissionApplicationResponse>>> GetAdmissionContractById
            ([FromQuery] int contractId)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }

                return await _contractService.GetAdmissionContractById(account.Id, contractId);
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
        /// Get Search Contract
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("search")]
        public async Task<ActionResult<BaseResponsePagingViewModel<AdmissionApplicationResponse>>> AdmissionSearchContract
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

                return await _contractService.AdmisionSearchContract(account.Id, search, paging);
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
        /// Get Contract By id
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost("create")]
        public async Task<ActionResult<BaseResponseViewModel<AdmissionApplicationResponse>>> CreateAdmissionContract
            ([FromBody] CreateAdmissionContractRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }

                return await _contractService.CreateAdmissionContract(account.Id, request);
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
        /// Update Contract
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPut("update")]
        public async Task<ActionResult<BaseResponseViewModel<AdmissionApplicationResponse>>> UpdateAdmissionContract
            ([FromQuery] int contractId, [FromBody] UpdateAdmissionContractRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }

                return await _contractService.UpdateAdmissionContract(account.Id, contractId, request);
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
        /// Send contract email to Collaborators
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost("sendContractEmail")]
        public async Task<ActionResult<BaseResponseViewModel<bool>>> AdmissionSendContractEmail([FromQuery] int contractId, [FromBody] List<int> collaboratorAccountId)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.AdmissionManager)
                {
                    return Unauthorized();
                }
                return await _contractService.AdmissionSendContractEmail(account.Id, contractId, collaboratorAccountId);
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
