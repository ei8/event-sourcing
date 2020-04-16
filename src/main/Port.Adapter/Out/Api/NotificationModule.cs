using Nancy;
using Nancy.Responses;
using Newtonsoft.Json;
using org.neurul.Common;
using works.ei8.EventSourcing.Application.Notifications;
using System.Linq;
using System.Text;
using works.ei8.EventSourcing.Common;

namespace works.ei8.EventSourcing.Port.Adapter.Out.Api
{
    public class NotificationModule : NancyModule
    {
        public NotificationModule(INotificationApplicationService notificationService) : base("/eventsourcing/notifications")
        {
            this.Get("/", async (parameters) => this.ProcessLog(
                await notificationService.GetCurrentNotificationLog(), 
                this.Request.Url.ToString()
                )
                );

            this.Get("/{logid}", async (parameters) => this.ProcessLog(
                await notificationService.GetNotificationLog(parameters.logid),
                this.Request.Url.ToString().Substring(
                    0, 
                    this.Request.Url.ToString().Length - parameters.logid.ToString().Length - 1
                    )
                )
                );
        }

        private TextResponse ProcessLog(NotificationLog log, string requestUrlBase)
        {
            var response = new TextResponse(JsonConvert.SerializeObject(log.NotificationList.ToArray()));
            var sb = new StringBuilder();
            ResponseHelper.Header.Link.AppendValue(
                sb,
                $"{requestUrlBase}/{log.NotificationLogId}", 
                org.neurul.Common.Constants.Response.Header.Link.Relation.Self
                );

            if (log.HasFirstNotificationLog)
                ResponseHelper.Header.Link.AppendValue(
                    sb,
                    $"{requestUrlBase}/{log.FirstNotificationLogId}",
                    org.neurul.Common.Constants.Response.Header.Link.Relation.First
                    );

            if (log.HasPreviousNotificationLog)
                ResponseHelper.Header.Link.AppendValue(
                    sb,
                    $"{requestUrlBase}/{log.PreviousNotificationLogId}", 
                    org.neurul.Common.Constants.Response.Header.Link.Relation.Previous
                    );

            if (log.HasNextNotificationLog)
                ResponseHelper.Header.Link.AppendValue(
                    sb,
                    $"{requestUrlBase}/{log.NextNotificationLogId}", 
                    org.neurul.Common.Constants.Response.Header.Link.Relation.Next
                    );

            response.Headers.Add(org.neurul.Common.Constants.Response.Header.Link.Key, sb.ToString());
            response.Headers.Add(org.neurul.Common.Constants.Response.Header.TotalCount.Key, log.TotalCount.ToString());
            return response;
        }
    }
}