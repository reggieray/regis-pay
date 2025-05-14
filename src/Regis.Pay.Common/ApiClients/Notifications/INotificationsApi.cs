using Refit;

namespace Regis.Pay.Common.ApiClients.Notifications
{
    public interface INotificationsApi
    {
        [Post("/notifications/api/send")]
        Task<HttpResponseMessage> SendNotificationAsync([Body] NotificationRequest request);
    }
}
