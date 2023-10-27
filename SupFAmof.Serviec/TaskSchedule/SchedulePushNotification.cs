using System;
using System.Linq;
using System.Text;
using Coravel.Invocable;
using SupFAmof.Data.Entity;
using System.Threading.Tasks;
using SupFAmof.Data.UnitOfWork;
using System.Collections.Generic;
using SupFAmof.Service.DTO.Request;
using static SupFAmof.Service.Utilities.Ultils;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof.Service.TaskSchedule
{
    public class SchedulePushNotification : IInvocable
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExpoTokenService notificationService;

        public SchedulePushNotification(IUnitOfWork unitOfWork,IExpoTokenService notificationService)
        {
            _unitOfWork = unitOfWork;
            this.notificationService = notificationService;
        }
        public async Task Invoke()
        {
            var dict = await GetAttendeeFromTime();
            if (dict != null) { 
                PushNotificationRequest request = new PushNotificationRequest
                {
                    Ids = dict.Keys.ToList(),
                    Body = dict.Values.First(),
                    Title = "Upcoming event"
                };
                await notificationService.PushNotification(request);
            }
        }

        private async Task<Dictionary<int,string>> GetAttendeeFromTime()
        {
            DateTime now = GetCurrentDatetime();
            Dictionary<int,string> Attendee = new Dictionary<int, string>();
            var list = _unitOfWork.Repository<PostAttendee>().GetAll()
                                    .Where(x => x.Post.DateFrom.Day.Equals(now.Day)).ToList();
            foreach ( var item in list )
            {
                Attendee.Add(item.AccountId, item.Post.PostCode);
            }
            return Attendee;
        }
    }
}
