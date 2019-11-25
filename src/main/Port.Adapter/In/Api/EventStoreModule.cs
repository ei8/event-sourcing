using Nancy;
using Nancy.Extensions;
using Nancy.IO;
using Newtonsoft.Json;
using org.neurul.Common.Events;
using works.ei8.EventSourcing.Application.EventStores;
using System.Collections.Generic;

namespace works.ei8.EventSourcing.Port.Adapter.In.Api
{
    public class EventStoreModule : NancyModule
    {
        public EventStoreModule(IEventStoreApplicationService eventStoreService) : base("/{avatarId}/eventsourcing/eventstore")
        {
            this.Post(string.Empty, async (parameters) =>
            {
                var notifs = JsonConvert.DeserializeObject<IEnumerable<Notification>>(RequestStream.FromStream(this.Request.Body).AsString());
                await eventStoreService.Save(parameters.avatarId, notifs);
            }
            );
        }
    }
}
