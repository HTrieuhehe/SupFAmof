using Expo.Server.Models;
using Microsoft.AspNetCore.Mvc;
using SupFAmof.Service.Service;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.Ocsp;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof.API.Controllers
{
    [Route(Helpers.SettingVersionAPI.ApiVersion)]
    [ApiController]
    public class NotiHistoryTestController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IExpoTokenService _expoTokenService;

        public NotiHistoryTestController(INotificationService notificationService, IExpoTokenService expoTokenService)
        {
            _notificationService = notificationService;
            _expoTokenService = expoTokenService;
        }

        /// <summary>
        /// Get Noti By Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("getNotiByToken")]
        public async Task<ActionResult<BaseResponsePagingViewModel<NotificationHistoryResponse>>> GetNotificationByToken([FromQuery] PagingRequest paging)
        {
            try
            {   
                //var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                //var account = FireBaseService.GetUserIdFromHeaderToken(accessToken);
                //if (account.Id == (int)SystemAuthorize.NotAuthorize || account.RoleId != (int)SystemRoleEnum.Collaborator)
                //{
                //    return Unauthorized();
                //}
                return await _notificationService.GetNotificationById(14, paging);
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
        [HttpPost("push-notification")]
        public async Task<ActionResult<BaseResponseViewModel<PushTicketResponse>>> PushNotification
          ([FromBody] PushNotificationRequest request)
        {
            try
            {
                var result = await _expoTokenService.PushNotification(request);
                return Ok(result);
            }
            catch (ErrorResponse ex)
            {
                return BadRequest(ex.Error);
            }
        }


    }
}
