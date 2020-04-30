using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ei8.EventSourcing.Common;

namespace ei8.EventSourcing.Application.EventStores
{
    public interface IEventStoreApplicationService
    {
        Task<IEnumerable<Notification>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default);

        Task Save(IEnumerable<Notification> notifications, CancellationToken cancellationToken = default);
    }
}
