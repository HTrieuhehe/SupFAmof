using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.Service;
using SupFAmof.Service.Service.ServiceInterface;
using static SupFAmof.Service.Helpers.Enum;

namespace SupFAmof.API.Controllers
{
    [Route(Helpers.SettingVersionAPI.ApiVersion)]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;

        public ContractController(IContractService contractService)
        {
            _contractService = contractService;
        }

        ///<summary>
        /// Get All contracts for testing
        /// </summary>
        /// 
        [HttpGet("getAll")]
        public async Task<ActionResult<BaseResponsePagingViewModel<AccountContractResponse>>> GetContracts([FromQuery] AccountContractResponse filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                //var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                //var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                //if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                //{
                //    return Unauthorized();
                //}
                return await _contractService.GetContracts(filter, paging);
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

        ///<summary>
        /// Get Contract By Account Id
        /// </summary>
        /// 
        [HttpGet("getById")]
        public async Task<ActionResult<BaseResponsePagingViewModel<AccountContractResponse>>> GetContractsByAccountId([FromQuery] AccountContractResponse filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                return await _contractService.GetContractsByAccountId(account.Id, paging);
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

        ///<summary>
        /// Confirm Contract By Account Id
        /// </summary>
        /// 
        [HttpPut("confirmContract")]
        public async Task<ActionResult<BaseResponseViewModel<AccountContractResponse>>> ConfirmContract
            ([FromQuery] int accountContractId, int status)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                return await _contractService.ConfirmContract(account.Id, accountContractId, status);
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
