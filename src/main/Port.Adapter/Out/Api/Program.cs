using ei8.EventSourcing.Application;
using ei8.EventSourcing.Application.EventStores;
using ei8.EventSourcing.Application.Notifications;
using ei8.EventSourcing.Common;
using ei8.EventSourcing.Port.Adapter.IO.Persistence.Events.SQLite;
using ei8.EventSourcing.Port.Adapter.IO.Process.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using neurUL.Common;
using System;
using System.Linq;
using System.Net;
using System.Text;
using dmIEventStore = ei8.EventSourcing.Domain.Model.IEventStore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection();

var sp = builder.Services.BuildServiceProvider();
builder.Services.AddSingleton(sp.GetRequiredService<IDataProtectionProvider>().CreateProtector(typeof(EventStore).FullName));

// Add services to the container.
builder.Services.AddSingleton<ISettingsService, SettingsService>();
builder.Services.AddScoped<dmIEventStore, EventStore>();
builder.Services.AddScoped<INotificationApplicationService, NotificationApplicationService>();
builder.Services.AddScoped<IEventStoreApplicationService, EventStoreApplicationService>();

builder.Services.AddHttpClient();

// Add swagger UI.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add background services.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

// Uncomment to use HTTPS.
//app.UseHttpsRedirection();

// Add endpoints here.
app.MapGet(
    "/eventsourcing/eventstore/{aggregateId}",
    async (
        [FromServices] IEventStoreApplicationService eventStoreService,
        HttpContext context,
        string aggregateId
    ) =>
    {
        int version = 0;
        if (context.Request.Query.ContainsKey("version"))
            version = int.Parse(context.Request.Query["version"]);
        return await eventStoreService.Get(Guid.Parse(aggregateId), version);
    }
);

app.MapGet(
    "/eventsourcing/notifications",
    async (
        [FromServices] INotificationApplicationService notificationService,
        HttpContext context
    ) =>
    {
        var result = await notificationService.GetCurrentNotificationLog();
        ProcessLog(
            context.Response,
            result,
            Microsoft.AspNetCore.Http.Extensions.UriHelper.GetEncodedUrl(context.Request)
        );
        return result.NotificationList.ToArray();
    }
);

app.MapGet(
    "/eventsourcing/notifications/{logId}",
    async (
        [FromServices] INotificationApplicationService notificationService,
        HttpContext context,
        string logId
    ) =>
    {
        var result = await notificationService.GetNotificationLog(logId);
        ProcessLog(
            context.Response,
            result,
            RemoveLogId(
                Microsoft.AspNetCore.Http.Extensions.UriHelper.GetEncodedUrl(context.Request),
                logId
            )
        );
        return result.NotificationList.ToArray();
    }
);

// Add global exception handling
app.UseExceptionHandler(appError =>
{
    appError.Run(async (context) =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var exceptionContext = context.Features.Get<IExceptionHandlerFeature>();

        if (exceptionContext != null)
            await context.Response.WriteAsync(exceptionContext.Error.ToString());
    });
});

app.Run();

static string RemoveLogId(string url, string logId)
{
    return url.ToString().Substring(0, url.Length - logId.ToString().Length - 1);
}

static void ProcessLog(HttpResponse response, NotificationLog log, string requestUrlBase)
{
    var sb = new StringBuilder();
    ResponseHelper.Header.Link.AppendValue(
        sb,
        $"{requestUrlBase}/{log.NotificationLogId}",
        neurUL.Common.Constants.Response.Header.Link.Relation.Self
        );

    if (log.HasFirstNotificationLog)
        ResponseHelper.Header.Link.AppendValue(
            sb,
            $"{requestUrlBase}/{log.FirstNotificationLogId}",
            neurUL.Common.Constants.Response.Header.Link.Relation.First
            );

    if (log.HasPreviousNotificationLog)
        ResponseHelper.Header.Link.AppendValue(
            sb,
            $"{requestUrlBase}/{log.PreviousNotificationLogId}",
            neurUL.Common.Constants.Response.Header.Link.Relation.Previous
            );

    if (log.HasNextNotificationLog)
        ResponseHelper.Header.Link.AppendValue(
            sb,
            $"{requestUrlBase}/{log.NextNotificationLogId}",
            neurUL.Common.Constants.Response.Header.Link.Relation.Next
            );

    response.Headers.Add(neurUL.Common.Constants.Response.Header.Link.Key, sb.ToString());
    response.Headers.Add(neurUL.Common.Constants.Response.Header.TotalCount.Key, log.TotalCount.ToString());
}