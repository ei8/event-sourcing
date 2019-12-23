using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using works.ei8.EventSourcing.Common;

namespace works.ei8.EventSourcing.Application.Notifications
{
    public interface INotificationApplicationService
    {
        Task<NotificationLog> GetCurrentNotificationLog(string storeId, CancellationToken cancellationToken = default);

        Task<NotificationLog> GetNotificationLog(string storeId, string notificationLogId, CancellationToken cancellationToken = default);
    }
}
