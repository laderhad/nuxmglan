using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NuxToneGenerator.Infrastructure;
using NuxToneGenerator.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors();
app.MapOpenApi();

// Register endpoints
app.MapToneEndpoints();
app.MapCatalogEndpoints();
app.MapPresetEndpoints();
app.MapPatchEndpoints();

app.Run();
