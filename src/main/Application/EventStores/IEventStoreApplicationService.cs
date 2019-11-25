using org.neurul.Common.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace works.ei8.EventSourcing.Application.EventStores
{
    public interface IEventStoreApplicationService
    {
        Task<IEnumerable<Notification>> Get(string storeId, Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default);

        Task Save(string storeId, IEnumerable<Notification> notifications, CancellationToken cancellationToken = default);
    }
}
