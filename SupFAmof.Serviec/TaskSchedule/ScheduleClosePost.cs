using System;
using System.Linq;
using System.Text;
using Coravel.Invocable;
using SupFAmof.Data.Entity;
using System.Threading.Tasks;
using SupFAmof.Data.UnitOfWork;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static SupFAmof.Service.Utilities.Ultils;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof.Service.TaskSchedule
{
    public class ScheduleClosePost : IInvocable
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPostService _postService;

        public ScheduleClosePost(IUnitOfWork unitOfWork, IPostService postService)
        {
            _unitOfWork = unitOfWork;
            _postService = postService;
        }

        public async Task Invoke()
        {
            var getPostIds = await _unitOfWork.Repository<Post>().GetWhere(x => x.DateFrom == GetCurrentDatetime());
            foreach (var post in getPostIds)
            {
                await _postService.ClosePostRegistration(post.AccountId,post.Id);
            }
        }
    }
}
