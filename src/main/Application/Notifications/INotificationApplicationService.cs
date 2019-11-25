using org.neurul.Common.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace works.ei8.EventSourcing.Application.Notifications
{
    public interface INotificationApplicationService
    {
        Task<NotificationLog> GetCurrentNotificationLog(string storeId, CancellationToken cancellationToken = default);

        Task<NotificationLog> GetNotificationLog(string storeId, string notificationLogId, CancellationToken cancellationToken = default);
    }
}
