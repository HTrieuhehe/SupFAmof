using System;
using System.Linq;
using System.Text;
using Coravel.Invocable;
using SupFAmof.Data.Entity;
using System.Threading.Tasks;
using SupFAmof.Data.UnitOfWork;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SupFAmof.Service.TaskSchedule
{
    public class SchedulePushNotification : IInvocable
    {
        private readonly IUnitOfWork _unitOfWork;

        public SchedulePushNotification(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task Invoke()
        {
            throw new NotImplementedException();
        }

        public async Task<Dictionary<int,List<Post>>> CollabAndPostThatNotRegistered()
        {
            var listCollaborator = _unitOfWork.Repository<Account>().GetAll()
                                                    .Where(x => x.Email.EndsWith("fpt.edu.vn"))
                                                     .Select(x => x.Id);                                                    ;
            var Posts = _unitOfWork.Repository<Post>().GetAll().Include(x=>x.PostAttendees).ToList();
            foreach (var post in Posts)
            {
                List<int> notJoinedByPost = (List<int>)post.PostAttendees.Select(x=>x.AccountId).Intersect(listCollaborator);
            }
            return null;
        }
    }
}
