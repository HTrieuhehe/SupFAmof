using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.Service;
using Microsoft.AspNetCore.Http;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Request.AccounBanking;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof.API.Controllers
{
    [Route(Helpers.SettingVersionAPI.ApiVersion)]
    [ApiController]
    public class AccountBankingController : ControllerBase
    {
        private readonly IAccountBankingService _accountBankingService;

        public AccountBankingController(IAccountBankingService accountBankingService)
        {
            _accountBankingService = accountBankingService;
        }

        [HttpGet("getById/accountBankingId")]
        public async Task<ActionResult<AccountBankingResponse>> GetAccountBankingById(int accountBankingId)
        {
            try
            {
                var result = await _accountBankingService.GetAccountBankingById(accountBankingId);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        [HttpGet("getAccountBanking")]
        public async Task<ActionResult<AccountBankingResponse>> GetAccountBankings([FromQuery] AccountBankingResponse request, [FromQuery] PagingRequest paging)
        {
            try
            {
                var result = await _accountBankingService.GetAccountBankings(request, paging);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        [HttpPost("createAccountBanking")]
        public async Task<ActionResult<AccountBankingResponse>> CreateAccountBanking([FromBody] CreateAccountBankingRequest request)
        {
            try
            {
                var result = await _accountBankingService.CreateAccountBanking(request);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        [HttpPut("updateAccountBanking/accountBankingId")]
        public async Task<ActionResult<AccountBankingResponse>> UpdateAccountBanking([FromQuery] int accountBankingId, [FromBody] UpdateAccountBankingRequest request)
        {
            try
            {
                var result = await _accountBankingService.UpdateAccountBanking(accountBankingId, request);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

    }
}
