using System;
using System.Linq;
using System.Text;
using Expo.Server.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using SupFAmof.Service.DTO.Request;
using SupFAmof.Service.DTO.Response;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IExpoTokenService
    {
        void AddFcmToken(string fcmToken, int customerId);

        void AddStaffFcmToken(string fcmToken, int staffId);

        int RemoveFcmTokens(ICollection<string> fcmTokens);

        void UnsubscribeAll(int accountId);

        void SubscribeAll(int accountId);

        Task<bool> ValidToken(string fcmToken);


        #region ExpoToken

        void AddExpoToken(string expoToken, int accountId);
        void AddAdminExpoToken(string expoToken, int adminId);
        Task<int> RemoveExpoTokens(ICollection<string> expoToken, int accountId, int status);
        Task<bool> ValidExpoToken(string expoToken, int accountId);
        Task<PushTicketResponse> PushNotification(PushNotificationRequest request);

        #endregion
    }
}
