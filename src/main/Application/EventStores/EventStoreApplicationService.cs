using org.neurul.Common.Domain.Model;
using org.neurul.Common.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<Notification>> Get(string storeId, Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default)
        {
            AssertionConcern.AssertArgumentNotEmpty(storeId, Constants.Messages.Exception.AvatarIdRequired, nameof(storeId));

            return await this.eventStore.Get(storeId, aggregateId, fromVersion, cancellationToken);
        }

        public async Task Save(string storeId, IEnumerable<Notification> notifications, CancellationToken cancellationToken = default)
        {
            AssertionConcern.AssertArgumentNotEmpty(storeId, Constants.Messages.Exception.AvatarIdRequired, nameof(storeId));

            await this.eventStore.Save(storeId, notifications, cancellationToken);
        }
    }
}
