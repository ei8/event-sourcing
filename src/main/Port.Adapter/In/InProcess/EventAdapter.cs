using ei8.EventSourcing.Application.EventStores;
using ei8.EventSourcing.Common;
using neurUL.Common.Domain.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.EventSourcing.Port.Adapter.In.InProcess
{
    public class EventAdapter : IEventAdapter
    {
        private readonly IEventStoreApplicationService eventStoreApplicationService;

        public EventAdapter(IEventStoreApplicationService eventStoreApplicationService)
        {
            AssertionConcern.AssertArgumentNotNull(eventStoreApplicationService, nameof(eventStoreApplicationService));

            this.eventStoreApplicationService = eventStoreApplicationService;
        }

        public async Task Save(IEnumerable<Notification> notifications, CancellationToken cancellationToken = default)
        {
            await this.eventStoreApplicationService.Save(notifications, cancellationToken);
        }
    }
}
