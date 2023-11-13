using System;
using System.Linq;
using System.Text;
using Coravel.Invocable;
using SupFAmof.Data.Entity;
using System.Threading.Tasks;
using SupFAmof.Data.UnitOfWork;
using System.Collections.Generic;
using SupFAmof.Service.Utilities;
using SupFAmof.Service.DTO.Request;
using static SupFAmof.Service.Helpers.Enum;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof.Service.TaskSchedule
{
    public class ScheduleNotificationPostReOpen : IInvocable
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public ScheduleNotificationPostReOpen(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task Invoke()
        {
            var getPostIds = await _unitOfWork.Repository<Post>().GetWhere(x => x.Status == (int)PostStatusEnum.Re_Open);
            var account = _unitOfWork.Repository<Account>().GetAll()
                                            .Where(x => x.IsActive == true && x.RoleId == (int)SystemRoleEnum.Collaborator
                                                                           || x.AccountBanneds.Any() // Đảm bảo có ít nhất một bản ghi AccountBanned
                                                                           && x.AccountBanneds.Max(b => b.DayEnd) <= Ultils.GetCurrentDatetime());

            var accountIds = account.Select(p => p.Id).ToList();
            if (getPostIds.Any())
            {
                PushNotificationRequest notificationRequest = new PushNotificationRequest()
                {
                    Ids = accountIds,
                    Title = NotificationTypeEnum.Post_Re_Opened.GetDisplayName(),
                    Body = $"There are {getPostIds.Count()} post re opened ! Apply now!",
                    NotificationsType = (int)NotificationTypeEnum.Post_Re_Opened
                };
                await _notificationService.PushNotification(notificationRequest);
            }
        }
    }
}
