using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface INotificationService
    {
        Task<BaseResponsePagingViewModel<NotificationHistoryResponse>> GetNotificationById(int recipientId, PagingRequest paging);
        Task<BaseResponsePagingViewModel<NotificationHistoryResponse>> GetNotifications(NotificationHistoryResponse filter, PagingRequest paging);
        Task<BaseResponseViewModel<NotificationHistoryResponse>> CreateNotification(CreateNotificationHistoryRequest request);
    }
}
