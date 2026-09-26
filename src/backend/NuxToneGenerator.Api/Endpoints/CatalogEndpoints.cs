namespace NuxToneGenerator.Api.Endpoints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using NuxToneGenerator.Core.Interfaces;

public static class CatalogEndpoints
{
    public static void MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/catalog", ([FromServices] IEquipmentCatalogService service) =>
        {
            return Results.Ok(service.GetCatalog());
        });
        
        app.MapGet("/api/catalog/amplifiers", ([FromServices] IEquipmentCatalogService service) =>
        {
            return Results.Ok(service.GetCatalog().Amplifiers);
        });
        
        app.MapGet("/api/catalog/effects/{category}", (string category, [FromServices] IEquipmentCatalogService service) =>
        {
            return Results.Ok(service.GetEffectsByCategory(category));
        });
    }
}
