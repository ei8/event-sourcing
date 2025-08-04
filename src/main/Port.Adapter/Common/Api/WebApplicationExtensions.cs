using ei8.EventSourcing.Application.Keys;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using neurUL.Common;
using System.Net;
using System.Text.Json;

namespace ei8.EventSourcing.Port.Adapter.Common.Api
{
    public static class WebApplicationExtensions
    {
        public static void AddKeyHandler(this WebApplication app)
        {
            app.MapPost("/eventsourcing/key", async (
                [FromServices] IKeyApplicationService keyApplicationService,
                HttpContext context
            ) =>
            {
                var result = Results.Ok();
                try
                {
                    var data = await new StreamReader(context.Request.Body).ReadToEndAsync();
                    var keys = JsonSerializer.Deserialize<IEnumerable<string>>(data);
                    await keyApplicationService.Load(keys);
                }
                catch (Exception ex)
                {
                    result = Results.Problem(ex.ToDetailedString(), statusCode: (int)HttpStatusCode.InternalServerError);
                }
                return result;
            });
        }
    }
}
