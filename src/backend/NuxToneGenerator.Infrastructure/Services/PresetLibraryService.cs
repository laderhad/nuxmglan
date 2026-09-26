namespace NuxToneGenerator.Infrastructure.Services;

using NuxToneGenerator.Core.Interfaces;
using NuxToneGenerator.Core.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public class PresetLibraryService : IPresetLibraryService
{
    private readonly ConcurrentDictionary<Guid, SavedPreset> _presets = new();
    private readonly string _filePath = "data/presets.json";

    public PresetLibraryService()
    {
        Directory.CreateDirectory("data");
        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);
            var list = JsonSerializer.Deserialize<List<SavedPreset>>(json) ?? new List<SavedPreset>();
            foreach (var p in list)
                _presets.TryAdd(p.Id, p);
        }
    }

    private void SaveToFile()
    {
        var json = JsonSerializer.Serialize(_presets.Values.ToList());
        File.WriteAllText(_filePath, json);
    }

    public Task<IReadOnlyList<SavedPreset>> GetAllAsync(CancellationToken ct = default)
    {
        return Task.FromResult<IReadOnlyList<SavedPreset>>(_presets.Values.ToList());
    }

    public Task<SavedPreset?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _presets.TryGetValue(id, out var preset);
        return Task.FromResult(preset);
    }

    public Task<SavedPreset> SaveAsync(SavedPreset preset, CancellationToken ct = default)
    {
        var p = preset with { UpdatedAt = DateTime.UtcNow };
        _presets[p.Id] = p;
        SaveToFile();
        return Task.FromResult(p);
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        if (_presets.TryRemove(id, out _))
        {
            SaveToFile();
        }
        return Task.CompletedTask;
    }
}
