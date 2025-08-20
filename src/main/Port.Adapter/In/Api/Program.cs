using ei8.EventSourcing.Application;
using ei8.EventSourcing.Application.EventStores;
using ei8.EventSourcing.Application.Keys;
using ei8.EventSourcing.Common;
using ei8.EventSourcing.Domain.Model;
using ei8.EventSourcing.Port.Adapter.Common.Api;
using ei8.EventSourcing.Port.Adapter.IO.Persistence.Events.SQLite;
using ei8.EventSourcing.Port.Adapter.IO.Process.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using neurUL.Common;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using dmIEventStore = ei8.EventSourcing.Domain.Model.IEventStore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDataProtection();

var sp = builder.Services.BuildServiceProvider();
builder.Services.AddSingleton(sp.GetRequiredService<IDataProtectionProvider>().CreateProtector(typeof(EventStore).FullName));

// Add services to the container.
builder.Services.AddSingleton<ISettingsService, SettingsService>();
builder.Services.AddScoped<dmIEventStore, EventStore>();
builder.Services.AddScoped<IEventStoreApplicationService, EventStoreApplicationService>();
builder.Services.AddSingleton<IKeyService, KeyService>();
builder.Services.AddScoped<IKeyApplicationService, KeyApplicationService>();

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
app.MapPost("/eventsourcing/eventstore", async (
    [FromServices] IEventStoreApplicationService eventStoreService, 
    HttpContext context
) =>
{
    var result = Results.Ok();
    try
    {
        var data = await new StreamReader(context.Request.Body).ReadToEndAsync();
        var notifs = JsonSerializer.Deserialize<IEnumerable<Notification>>(data);
        await eventStoreService.Save(notifs);
    }
    catch (Exception ex)
    {
        HttpStatusCode hsc = HttpStatusCode.InternalServerError;

        if (ex is SQLiteException sle && sle.Message.Contains("Constraint"))
            hsc = HttpStatusCode.Conflict;

        result = Results.Problem(ex.ToDetailedString(), statusCode: (int) hsc);
    }
    return result;
});

app.AddKeyHandler();

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