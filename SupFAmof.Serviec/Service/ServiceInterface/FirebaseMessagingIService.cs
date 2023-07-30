using FirebaseAdmin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupFAmof.Service.Service.ServiceInterface
{
    public interface IFirebaseMessagingService
    {
        void SendToTopic(string topic, Notification notification, Dictionary<string, string> data);
        void SendToTopic(string topic, Dictionary<string, string> data);
        void SendToTopicAsync(string topic, Notification notification);
        void SendToDevices(List<string> tokens, Notification notification, Dictionary<string, string> data);
        void SendToDevices(List<string> tokens, Dictionary<string, string> data);
        void SendToDevices(List<string> tokens, Notification notification);
        void Subcribe(IReadOnlyList<string> tokens, string topic);
        Task<bool> ValidToken(string fcmToken);
        void Unsubcribe(IReadOnlyList<string> tokens, string topic);
    }
}
