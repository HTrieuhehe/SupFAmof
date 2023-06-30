using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.Service;
using Microsoft.AspNetCore.Http;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.DTO.Request.AccounBanking;

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

        [HttpGet("/GetAccountBankingById/accountId")]
        public async Task<ActionResult<AccountBankingResponse>> GetAccountBankingById(int accountId)
        {
            try
            {
                var result = await _accountBankingService.GetAccountBankingById(accountId);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
        [HttpGet("/GetAccountBanking")]
        public async Task<ActionResult<AccountBankingResponse>> GetAccountBankings([FromQuery] AccountBankingResponse request, [FromQuery] PagingRequest paging)
        {
            try
            {
                var result = await _accountBankingService.GetAccountBankings(request,paging);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        [HttpPost("/CreateAccountBanking")]
        public async Task<ActionResult<AccountBankingResponse>> CreateAccountBanking(CreateAccountBankingRequest request)
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
        [HttpPut("/UpdateAccountBanking/accountId")]
        public async Task<ActionResult<AccountBankingResponse>> UpdateAccountBanking(int accountId, UpdateAccountBankingRequest request)
        {
            try
            {
                var result = await _accountBankingService.UpdateAccountBanking(accountId,request);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

    }
}
