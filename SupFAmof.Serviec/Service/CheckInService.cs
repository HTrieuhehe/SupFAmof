using System;
using AutoMapper;
using System.Linq;
using System.Text;
using ServiceStack.Text;
using Google.Api.Gax.Rest;
using SupFAmof.Data.Entity;
using MimeKit.Cryptography;
using System.Threading.Tasks;
using SupFAmof.Data.UnitOfWork;
using System.Collections.Generic;
using SupFAmof.Service.Exceptions;
using SupFAmof.Service.DTO.Request;
using Microsoft.EntityFrameworkCore;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof.Service.Service
{
    public class CheckInService : ICheckInService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CheckInService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CheckIn(CheckInRequest checkin)
        {
            try
            {
                var checkAttendance = _mapper.Map<CheckAttendance>(checkin);
                var postVerification = await _unitOfWork.Repository<PostAttendee>().GetAll().SingleOrDefaultAsync(x => x.PostId == checkin.PostId && x.PositionId == checkin.PositionId && x.AccountId == checkin.AccountId);
                double distance = 0.04; // kilometer 
                double userCurrentPosition = ((double)Utilities.Ultils.CalculateDistance(postVerification.Position.Latitude, postVerification.Position.Longtitude,checkin.Latitude,checkin.Longtitude));
                if(userCurrentPosition > distance)
                {
                    throw new ErrorResponse(400,400,$"Distance { userCurrentPosition } is to far ");
                }
                if(VerifyDateTimeCheckin(postVerification,checkin.CheckInTime))
                {
                    await _unitOfWork.Repository<CheckAttendance>().InsertAsync(checkAttendance);
                    await _unitOfWork.CommitAsync();
                }

            }catch(Exception ex)
            {
                throw;
            }
        }
        private bool VerifyDateTimeCheckin(PostAttendee postTime,DateTime checkInTime)
        {
            if (postTime.Post.DateFrom.Date != checkInTime.Date)
            {
                return false; // Different days
            }
            
            // Calculate the time difference in hours between checkInTime and postTime.Position.TimeFrom
            TimeSpan timeDifference = checkInTime.TimeOfDay - postTime.Position.TimeFrom;

            // Check if the time difference is within a 2-hour range
            if (timeDifference.TotalHours < -2 || timeDifference.TotalHours > 2)
            {
                return false; // Outside the 2-hour range
            }

            return true;
        }

    }

}
