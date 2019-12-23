using Nancy;
using Nancy.Extensions;
using Nancy.IO;
using Newtonsoft.Json;
using works.ei8.EventSourcing.Application.EventStores;
using System.Collections.Generic;
using System;
using Nancy.Responses;
using org.neurul.Common;
using SQLite;
using works.ei8.EventSourcing.Common;

namespace works.ei8.EventSourcing.Port.Adapter.In.Api
{
    public class EventStoreModule : NancyModule
    {
        public EventStoreModule(IEventStoreApplicationService eventStoreService) : base("/{avatarId}/eventsourcing/eventstore")
        {
            this.Post(string.Empty, async (parameters) =>
            {
                var result = new Response { StatusCode = HttpStatusCode.OK };

                try
                {
                    var notifs = JsonConvert.DeserializeObject<IEnumerable<Notification>>(RequestStream.FromStream(this.Request.Body).AsString());
                    await eventStoreService.Save(parameters.avatarId, notifs);
                }
                catch (Exception ex)
                {
                    HttpStatusCode hsc = HttpStatusCode.BadRequest;

                    if (ex is SQLiteException sle && sle.Message.Contains("Constraint"))
                        hsc = HttpStatusCode.Conflict;

                    result = new TextResponse(hsc, ex.ToDetailedString());
                }

                return result;
            }
            );
        }
    }
}
