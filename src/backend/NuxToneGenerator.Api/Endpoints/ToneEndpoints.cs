namespace NuxToneGenerator.Api.Endpoints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using NuxToneGenerator.Core.Interfaces;
using NuxToneGenerator.Core.Models;

public static class ToneEndpoints
{
    public static void MapToneEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tone/generate", async ([FromBody] ToneRequest request, [FromServices] IToneGenerationService service) =>
        {
            var response = await service.GenerateToneAsync(request);
            return Results.Ok(response);
        });
    }
}
