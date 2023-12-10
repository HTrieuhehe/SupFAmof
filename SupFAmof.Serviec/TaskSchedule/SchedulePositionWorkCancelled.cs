using System;
using System.Linq;
using System.Text;
using Coravel.Invocable;
using SupFAmof.Data.Entity;
using System.Threading.Tasks;
using SupFAmof.Data.UnitOfWork;
using SupFAmof.Service.Helpers;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using static SupFAmof.Service.Helpers.Enum;
using static SupFAmof.Service.Utilities.Ultils;

namespace SupFAmof.Service.TaskSchedule
{
    public class SchedulePositionWorkCancelled : IInvocable
    {
        private readonly IUnitOfWork _unitOfWork;

        public SchedulePositionWorkCancelled(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        //public async Task Invoke()
        //{
        //    DateTime currentDateTime = GetCurrentDatetime();

        //    // Fetch all necessary data upfront
        //    var posts = _unitOfWork.Repository<Post>()
        //        .GetAll()
        //        .Where(x => x.DateFrom <= currentDateTime && currentDateTime <= x.DateTo);

        //    var allCheckAttendances = _unitOfWork.Repository<CheckAttendance>()
        //        .GetAll();

        //    var allPostRegistrations = _unitOfWork.Repository<PostRegistration>()
        //        .GetAll()
        //        .Where(x => x.Status == (int)PostRegistrationStatusEnum.Confirm || x.Status == (int)PostRegistrationStatusEnum.CheckIn);

        //    foreach (var post in posts)
        //    {
        //        DateTime checkTimeCheckIn = GetCurrentDatetime();
        //        var checkTimeOfDay = checkTimeCheckIn.TimeOfDay;

        //        foreach (var position in post.PostPositions)
        //        {
        //            TimeSpan positionCheckInTime = position.TimeFrom + TimeSpan.FromMinutes(5);
        //            if (positionCheckInTime < checkTimeOfDay&&position.Date <= checkTimeCheckIn.Date)
        //            {
        //                var postRegistrationsConfirm = allPostRegistrations
        //                    .Where(x => x.PositionId == position.Id);

        //                var joinedPostRegistrations = postRegistrationsConfirm
        //                    .Select(postRegistration => new
        //                    {
        //                        PostRegistration = postRegistration,
        //                        IsCheckedIn = allCheckAttendances
        //                            .Any(checkAttendance => checkAttendance.PostRegistrationId == postRegistration.Id)
        //                    });

        //                foreach (var checkedPostRegistration in joinedPostRegistrations)
        //                {
        //                    if (!checkedPostRegistration.IsCheckedIn)
        //                    {
        //                        checkedPostRegistration.PostRegistration.Status = (int)PostRegistrationStatusEnum.Cancel;
        //                        checkedPostRegistration.PostRegistration.CancelTime = GetCurrentDatetime();
        //                        await _unitOfWork.Repository<PostRegistration>().UpdateDetached(checkedPostRegistration.PostRegistration);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    // Commit changes outside the loop
        //    await _unitOfWork.CommitAsync();
        //}
        public async Task Invoke()
        {
            DateTime currentDateTime = GetCurrentDatetime();

            // Fetch all necessary data upfront
            var posts = _unitOfWork.Repository<Post>()
                .GetAll()
                .Where(x => x.DateFrom <= currentDateTime && currentDateTime <= x.DateTo);

            var allCheckAttendances = _unitOfWork.Repository<CheckAttendance>()
                .GetAll();

            var allPostRegistrations = _unitOfWork.Repository<PostRegistration>()
                .GetAll()
                .Where(x => x.Status == (int)PostRegistrationStatusEnum.Confirm || x.Status == (int)PostRegistrationStatusEnum.CheckIn);

            foreach (var post in posts)
            {
                DateTime checkTimeCheckIn = GetCurrentDatetime();
                var checkTimeOfDay = checkTimeCheckIn.TimeOfDay;

                foreach (var position in post.PostPositions)
                {
                    TimeSpan positionCheckInTime = position.TimeFrom + TimeSpan.FromMinutes(5);

                    if (position.Date < checkTimeCheckIn.Date)
                    {
                        await CancelUncheckedPostRegistrations(position, allPostRegistrations, allCheckAttendances);
                    }
                    else if (position.Date == checkTimeCheckIn.Date && positionCheckInTime <= checkTimeOfDay)
                    {
                        await CancelUncheckedPostRegistrations(position, allPostRegistrations, allCheckAttendances);

                    }

                }
            }

            // Commit changes outside the loop
            await _unitOfWork.CommitAsync();
        }
        private async Task CancelUncheckedPostRegistrations(PostPosition position, IEnumerable<PostRegistration> allPostRegistrations, IEnumerable<CheckAttendance> allCheckAttendances)
        {
            var postRegistrationsConfirm = allPostRegistrations
                .Where(x => x.PositionId == position.Id);

            var joinedPostRegistrations = postRegistrationsConfirm
                .Select(postRegistration => new
                {
                    PostRegistration = postRegistration,
                    IsCheckedIn = allCheckAttendances
                        .Any(checkAttendance => checkAttendance.PostRegistrationId == postRegistration.Id)
                });

            foreach (var checkedPostRegistration in joinedPostRegistrations)
            {
                if (!checkedPostRegistration.IsCheckedIn)
                {
                    checkedPostRegistration.PostRegistration.Status = (int)PostRegistrationStatusEnum.Cancel;
                    checkedPostRegistration.PostRegistration.CancelTime = GetCurrentDatetime();
                    await _unitOfWork.Repository<PostRegistration>().UpdateDetached(checkedPostRegistration.PostRegistration);
                }
            }
        }

    }
}
