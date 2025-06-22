using ei8.EventSourcing.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.EventSourcing.Port.Adapter.Out.InProcess
{
    public interface IEventAdapter
    {
        Task<IEnumerable<Notification>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default);
    }
}
