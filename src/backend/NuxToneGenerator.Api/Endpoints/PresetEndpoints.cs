namespace NuxToneGenerator.Api.Endpoints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using NuxToneGenerator.Core.Interfaces;
using System;

public static class PresetEndpoints
{
    public static void MapPresetEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/presets", async ([FromServices] IPresetLibraryService service) =>
        {
            return Results.Ok(await service.GetAllAsync());
        });
        
        app.MapGet("/api/presets/{id}", async (Guid id, [FromServices] IPresetLibraryService service) =>
        {
            var preset = await service.GetByIdAsync(id);
            return preset is not null ? Results.Ok(preset) : Results.NotFound();
        });
        
        app.MapPost("/api/presets", async ([FromBody] SavedPreset preset, [FromServices] IPresetLibraryService service) =>
        {
            return Results.Ok(await service.SaveAsync(preset));
        });
        
        app.MapDelete("/api/presets/{id}", async (Guid id, [FromServices] IPresetLibraryService service) =>
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        });
    }
}
