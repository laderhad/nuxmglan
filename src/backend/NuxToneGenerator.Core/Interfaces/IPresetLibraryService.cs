namespace NuxToneGenerator.Core.Interfaces;

using NuxToneGenerator.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public interface IPresetLibraryService
{
    Task<IReadOnlyList<SavedPreset>> GetAllAsync(CancellationToken ct = default);
    Task<SavedPreset?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<SavedPreset> SaveAsync(SavedPreset preset, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public record SavedPreset
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = string.Empty;
    public string OriginalPrompt { get; init; } = string.Empty;
    public Preset Preset { get; init; } = new();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
}
