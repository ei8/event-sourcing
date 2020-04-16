using org.neurul.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using works.ei8.EventSourcing.Common;
using dmIEventStore = works.ei8.EventSourcing.Domain.Model.IEventStore;

namespace works.ei8.EventSourcing.Application.EventStores
{
    public class EventStoreApplicationService : IEventStoreApplicationService
    {
        public EventStoreApplicationService(dmIEventStore eventStore)
        {
            this.eventStore = eventStore;
        }

        private readonly dmIEventStore eventStore;

        public async Task<IEnumerable<Notification>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default)
        {
            return await this.eventStore.Get(aggregateId, fromVersion, cancellationToken);
        }

        public async Task Save(IEnumerable<Notification> notifications, CancellationToken cancellationToken = default)
        {
            await this.eventStore.Save(notifications, cancellationToken);
        }
    }
}
