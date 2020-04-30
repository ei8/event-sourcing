using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ei8.EventSourcing.Common;

namespace ei8.EventSourcing.Domain.Model
{
    public interface IEventStore
    {
        Task<NotificationLog> GetLog(CancellationToken cancellationToken = default);

        Task<NotificationLog> GetLog(NotificationLogId logId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Notification>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default);

        Task Save(IEnumerable<Notification> notifications, CancellationToken cancellationToken = default);
    }
}
