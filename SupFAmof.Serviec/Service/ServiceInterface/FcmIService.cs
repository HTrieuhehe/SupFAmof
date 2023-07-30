using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IFcmTokenService
    {
        void AddFcmToken(string fcmToken, int customerId);

        void AddStaffFcmToken(string fcmToken, int staffId);

        int RemoveFcmTokens(ICollection<string> fcmTokens);

        void UnsubscribeAll(int accountId);

        void SubscribeAll(int accountId);

        Task<bool> ValidToken(string fcmToken);
    }
}
