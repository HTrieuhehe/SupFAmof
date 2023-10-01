using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof.API.Controllers.AdmissionController
{
    [Route(Helpers.SettingVersionAPI.ApiAdminVersion)]
    [ApiController]
    public class AdmissionBanAccountController : ControllerBase
    {
        private readonly IAccountBannedService _accountBannedService;

        public AdmissionBanAccountController(IAccountBannedService accountBannedService)
        {
            _accountBannedService = accountBannedService;
        }


        // code here
    }
}
