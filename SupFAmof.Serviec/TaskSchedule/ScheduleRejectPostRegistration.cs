using System;
using System.Linq;
using System.Text;
using Coravel.Invocable;
using SupFAmof.Data.Entity;
using System.Threading.Tasks;
using SupFAmof.Data.UnitOfWork;
using System.Collections.Generic;
using static SupFAmof.Service.Helpers.Enum;
using static SupFAmof.Service.Utilities.Ultils;

namespace SupFAmof.Service.TaskSchedule
{
    public class ScheduleRejectPostRegistration : IInvocable
    {
        private readonly IUnitOfWork _unitOfWork;

        public ScheduleRejectPostRegistration(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Invoke()
        {
            var getAllPostRegistration = await _unitOfWork.Repository<PostRegistration>().GetWhere(x=>x.Status == (int)PostRegistrationStatusEnum.Pending);
            var checkTime = GetCurrentDatetime();
            foreach(var postRegistration in getAllPostRegistration)
            {
                if (postRegistration.Position.Date == checkTime.Date && postRegistration.Position.TimeFrom < checkTime.TimeOfDay)
                {
                    postRegistration.Status = (int)PostRegistrationStatusEnum.Reject;
                    postRegistration.UpdateAt = checkTime;
                    await _unitOfWork.Repository<PostRegistration>().UpdateDetached(postRegistration);
                    await _unitOfWork.CommitAsync();
                }
                if(postRegistration.Position.Date < checkTime.Date)
                {
                    postRegistration.Status = (int)PostRegistrationStatusEnum.Reject;
                    postRegistration.UpdateAt = checkTime;
                    await _unitOfWork.Repository<PostRegistration>().UpdateDetached(postRegistration);
                    await _unitOfWork.CommitAsync();
                }
             
            }

        }
    }
}
