using System;
using System.Linq;
using System.Text;
using ServiceStack;
using Coravel.Invocable;
using SupFAmof.Data.Entity;
using System.Threading.Tasks;
using SupFAmof.Data.UnitOfWork;
using System.Collections.Generic;
using static SupFAmof.Service.Helpers.Enum;
using static SupFAmof.Service.Utilities.Ultils;

namespace SupFAmof.Service.TaskSchedule
{
    public class ScheduleCloseInterview : IInvocable
    {
        private readonly IUnitOfWork _unitOfWork;

        public ScheduleCloseInterview(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Invoke()
        {
            var getAllInterview = await _unitOfWork.Repository<TrainingEventDay>().GetWhere(x => x.Status == (int)TrainingEventDayStatusEnum.Create);
            var currentTime = GetCurrentDatetime();
            var filterSameDay = getAllInterview.Where(x=>x.Date == currentTime.Date);
            var filterLessDay = getAllInterview.Where(x=>x.Date < currentTime.Date);
            if(filterLessDay.Any())
            {
                foreach(var interview in filterLessDay) {
                    interview.Status = (int)TrainingEventDayStatusEnum.Complete;
                    await _unitOfWork.Repository<TrainingEventDay>().UpdateDetached(interview);
                }
            }
            if(filterSameDay.Any())
            {
                foreach (var interview in filterSameDay)
                {
                    if(interview.TimeTo < currentTime.TimeOfDay)
                    {
                        interview.Status = (int)TrainingEventDayStatusEnum.Complete;
                        await _unitOfWork.Repository<TrainingEventDay>().UpdateDetached(interview);

                    }
                }
            }
            await _unitOfWork.CommitAsync();

        }
    }
}
