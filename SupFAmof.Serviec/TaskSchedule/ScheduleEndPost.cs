using System;
using System.Linq;
using System.Text;
using Coravel.Invocable;
using SupFAmof.Data.Entity;
using System.Threading.Tasks;
using SupFAmof.Data.UnitOfWork;
using System.Collections.Generic;
using static SupFAmof.Service.Utilities.Ultils;
using SupFAmof.Service.Service.ServiceInterface;

namespace SupFAmof.Service.TaskSchedule
{
    public class ScheduleEndPost : IInvocable
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPostService _postService;

        public ScheduleEndPost(IUnitOfWork unitOfWork, IPostService postService)
        {
            _unitOfWork = unitOfWork;
            _postService = postService;
        }

        public async Task Invoke()
        {
            DateTime currentTime = GetCurrentDatetime();
            var getPosts = await _unitOfWork.Repository<Post>().GetWhere(x => x.DateTo == currentTime.Date);
            foreach (var post in getPosts)
            {
                if (post.PostPositions.Any(x=>x.Date == currentTime.Date))
                {
                    var check = await _unitOfWork.Repository<PostPosition>().GetWhere(x => x.Date == currentTime);
                    TimeSpan? maxTime = TimeSpan.MinValue;

                    foreach (var postPosition in check)
                    {
                        if (postPosition.TimeTo > maxTime)
                        {
                            maxTime = postPosition.TimeTo;
                        }
                    }
                    if (maxTime < currentTime.TimeOfDay)
                    {
                        await _postService.EndPost(post.AccountId, post.Id);
                    }

                }else
                {
                    await _postService.EndPost(post.AccountId, post.Id);
                }

            }
        }
    }
}
