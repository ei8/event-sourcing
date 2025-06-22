using ei8.EventSourcing.Application.EventStores;
using ei8.EventSourcing.Common;
using neurUL.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.EventSourcing.Port.Adapter.Out.InProcess
{
    public class EventAdapter : IEventAdapter
    {
        private readonly IEventStoreApplicationService eventStoreApplicationService;

        public EventAdapter(IEventStoreApplicationService eventStoreApplicationService)
        {
            AssertionConcern.AssertArgumentNotNull(eventStoreApplicationService, nameof(eventStoreApplicationService));

            this.eventStoreApplicationService = eventStoreApplicationService;
        }

        public async Task<IEnumerable<Notification>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default)
        {
            return await this.eventStoreApplicationService.Get(aggregateId, fromVersion, cancellationToken);
        }
    }
}
