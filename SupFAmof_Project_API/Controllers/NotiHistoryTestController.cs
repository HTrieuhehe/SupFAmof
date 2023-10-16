using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.DTO.Response;
using SupFAmof.Service.Service.ServiceInterface;
using SupFAmof.Service.Service;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.Exceptions;

namespace SupFAmof.API.Controllers
{
    [Route(Helpers.SettingVersionAPI.ApiVersion)]
    [ApiController]
    public class NotiHistoryTestController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotiHistoryTestController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Get Noti By Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("getNotiById")]
        public async Task<ActionResult<BaseResponsePagingViewModel<NotificationHistoryResponse>>> GetNotificationByToken([FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                return await _notificationService.GetNotificationById(account.Id, paging);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Get Noti 
        /// </summary>
        /// <returns></returns>
        [HttpGet("getNotiById")]
        public async Task<ActionResult<BaseResponsePagingViewModel<NotificationHistoryResponse>>> GetNotifications
            ([FromQuery] NotificationHistoryResponse filter, [FromQuery] PagingRequest paging)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                return await _notificationService.GetNotifications(filter, paging);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }

        /// <summary>
        /// Create Noti 
        /// </summary>
        /// <returns></returns>
        [HttpPost("create")]
        public async Task<ActionResult<BaseResponseViewModel<NotificationHistoryResponse>>> GetNotifications
            ([FromBody] CreateNotificationHistoryRequest request)
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                {
                    return Unauthorized();
                }
                return await _notificationService.CreateNotification(request);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }
    }
}
