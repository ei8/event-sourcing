using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using works.ei8.EventSourcing.Common;

namespace works.ei8.EventSourcing.Domain.Model
{
    public interface IEventStore
    {
        Task<NotificationLog> GetLog(string storeId, CancellationToken cancellationToken = default);

        Task<NotificationLog> GetLog(string storeId, NotificationLogId logId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Notification>> Get(string storeId, Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default);

        Task Save(string storeId, IEnumerable<Notification> notifications, CancellationToken cancellationToken = default);
    }
}
