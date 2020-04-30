using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ei8.EventSourcing.Common;

namespace ei8.EventSourcing.Application.Notifications
{
    public interface INotificationApplicationService
    {
        Task<NotificationLog> GetCurrentNotificationLog(CancellationToken cancellationToken = default);

        Task<NotificationLog> GetNotificationLog(string notificationLogId, CancellationToken cancellationToken = default);
    }
}
