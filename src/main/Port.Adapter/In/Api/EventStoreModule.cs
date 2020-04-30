using Nancy;
using Nancy.Extensions;
using Nancy.IO;
using Newtonsoft.Json;
using ei8.EventSourcing.Application.EventStores;
using System.Collections.Generic;
using System;
using Nancy.Responses;
using neurUL.Common;
using SQLite;
using ei8.EventSourcing.Common;

namespace ei8.EventSourcing.Port.Adapter.In.Api
{
    public class EventStoreModule : NancyModule
    {
        public EventStoreModule(IEventStoreApplicationService eventStoreService) : base("/eventsourcing/eventstore")
        {
            this.Post(string.Empty, async (parameters) =>
            {
                var result = new Response { StatusCode = HttpStatusCode.OK };

                try
                {
                    var notifs = JsonConvert.DeserializeObject<IEnumerable<Notification>>(RequestStream.FromStream(this.Request.Body).AsString());
                    await eventStoreService.Save(notifs);
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
