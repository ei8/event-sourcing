using Nancy;
using Nancy.TinyIoc;
using works.ei8.EventSourcing.Application.EventStores;
using works.ei8.EventSourcing.Application.Notifications;
using works.ei8.EventSourcing.Port.Adapter.IO.Persistence.Events.SQLite;
using dmIEventStore = works.ei8.EventSourcing.Domain.Model.IEventStore;

namespace works.ei8.EventSourcing.Port.Adapter.In.Api
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        public CustomBootstrapper()
        {
        }

        protected override void ConfigureRequestContainer(TinyIoCContainer container, NancyContext context)
        {
            base.ConfigureRequestContainer(container, context);

            container.Register<dmIEventStore, EventStore>();
            container.Register<IEventStoreApplicationService, EventStoreApplicationService>();
        }
    }
}
