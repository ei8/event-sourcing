using Nancy;
using Nancy.TinyIoc;
using ei8.EventSourcing.Application.EventStores;
using ei8.EventSourcing.Application.Notifications;
using ei8.EventSourcing.Port.Adapter.IO.Persistence.Events.SQLite;
using dmIEventStore = ei8.EventSourcing.Domain.Model.IEventStore;

namespace ei8.EventSourcing.Port.Adapter.Out.Api
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
            container.Register<INotificationApplicationService, NotificationApplicationService>();
            container.Register<IEventStoreApplicationService, EventStoreApplicationService>();
        }
    }
}
