namespace NuxToneGenerator.Infrastructure;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NuxToneGenerator.Core.Interfaces;
using NuxToneGenerator.Infrastructure.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IEquipmentCatalogService, EquipmentCatalogService>();
        services.AddScoped<IToneGenerationService, ToneGenerationService>();
        services.AddSingleton<IPatchSerializer, PatchSerializer>();
        services.AddSingleton<IPresetLibraryService, PresetLibraryService>();
        return services;
    }
}
