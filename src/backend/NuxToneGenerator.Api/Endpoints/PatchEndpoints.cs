namespace NuxToneGenerator.Api.Endpoints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using NuxToneGenerator.Core.Interfaces;
using NuxToneGenerator.Core.Models;
using System.IO;

public static class PatchEndpoints
{
    public static void MapPatchEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/patch/export", ([FromBody] Preset preset, [FromServices] IPatchSerializer service) =>
        {
            var data = service.Serialize(preset);
            return Results.File(data, "application/octet-stream", "patch.mg30patch");
        });
        
        app.MapPost("/api/patch/import", async (HttpRequest request, [FromServices] IPatchSerializer service) =>
        {
            using var ms = new MemoryStream();
            await request.Body.CopyToAsync(ms);
            var data = ms.ToArray();
            var presets = service.DeserializeAll(data);
            return Results.Ok(presets);
        });
        
        app.MapPost("/api/patch/export-json", ([FromBody] Preset preset) =>
        {
            return Results.Json(preset);
        });
    }
}
