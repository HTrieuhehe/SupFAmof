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
        private readonly INotificationService _notificationService;

        public SchedulePushNotification(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }
        public async Task Invoke()
        {
            var dict = await GetAttendeeFromTime();
            if (dict != null) { 
                PushNotificationRequest request = new PushNotificationRequest
                {
                    Ids = dict.Keys.ToList(),
                    Body = $"Upcoming event incoming {dict.Values.First()}",
                    Title = "Reminder"
                };
                await _notificationService.PushNotification(request);
            }
        }

        private async Task<Dictionary<int,string>> GetAttendeeFromTime()
        {
            DateTime now = GetCurrentDatetime();
            DateTime nextDay = now.AddDays(1);
            Dictionary<int,string> attendee = new Dictionary<int, string>();


            var list = _unitOfWork.Repository<PostRegistration>().GetAll()
                                    .Where(x => x.Position.Post.DateFrom.Day.Equals(nextDay.Day)).ToList();
            foreach (var item in list)
            {
                attendee.Add(item.AccountId, item.Position.Post.DateFrom.Date.ToString("MM/dd/yyyy"));
            }
            return attendee;
        }
    }
}
