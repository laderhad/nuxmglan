import os
import shutil

backend_dir = "/Users/kerem/Documents/nuxmglan/src/backend"

# Remove Class1.cs
for proj in ["NuxToneGenerator.Core", "NuxToneGenerator.Infrastructure"]:
    class1 = os.path.join(backend_dir, proj, "Class1.cs")
    if os.path.exists(class1):
        os.remove(class1)

# Ensure directories exist
os.makedirs(os.path.join(backend_dir, "NuxToneGenerator.Core", "Models"), exist_ok=True)
os.makedirs(os.path.join(backend_dir, "NuxToneGenerator.Core", "Interfaces"), exist_ok=True)
os.makedirs(os.path.join(backend_dir, "NuxToneGenerator.Infrastructure", "Services"), exist_ok=True)
os.makedirs(os.path.join(backend_dir, "NuxToneGenerator.Api", "Endpoints"), exist_ok=True)

# Define file contents

core_models_effecttype = """namespace NuxToneGenerator.Core.Models;

public enum EffectBlockType
{
    WahVolume,
    Compression,
    Overdrive,
    Amplifier,
    EQ,
    NoiseGate,
    Modulation,
    Delay,
    Reverb,
    EFX,
    CabinetIR
}
"""

core_models_preset = """namespace NuxToneGenerator.Core.Models;

using System;
using System.Collections.Generic;

public record Preset
{
    public int Index { get; init; }
    public string Name { get; init; } = string.Empty;
    public Scene SceneA { get; init; } = new();
    public Scene SceneB { get; init; } = new();
    public Scene SceneC { get; init; } = new();
    public CabinetIR Cabinet { get; init; } = new();
    public byte[] ImpulseResponseData { get; init; } = Array.Empty<byte>();
    public byte[] FrequencyResponseData { get; init; } = Array.Empty<byte>();
    public byte[] RawSceneData { get; init; } = Array.Empty<byte>();
}

public record Scene
{
    public AmplifierSettings Amplifier { get; init; } = new();
    public EffectSettings Compression { get; init; } = new();
    public EffectSettings Overdrive { get; init; } = new();
    public EffectSettings Modulation { get; init; } = new();
    public EffectSettings Delay { get; init; } = new();
    public EffectSettings Reverb { get; init; } = new();
    public EffectSettings NoiseGate { get; init; } = new();
    public EffectSettings EQ { get; init; } = new();
    public EffectSettings EFX { get; init; } = new();
    public EffectSettings WahVolume { get; init; } = new();
    public int[] SignalChain { get; init; } = new[] { 1, 5, 2, 3, 9, 4, 10, 6, 7, 8, 11 };
    public string PresetName { get; init; } = string.Empty;
    public int MasterLevel { get; init; } = 100;
    public int Tempo { get; init; } = 120;
    public byte[] RawData { get; init; } = Array.Empty<byte>();
}

public record AmplifierSettings
{
    public string ModelId { get; init; } = string.Empty;
    public string ModelName { get; init; } = string.Empty;
    public bool Enabled { get; init; } = true;
    public int Gain { get; init; } = 50;
    public int Bass { get; init; } = 50;
    public int Middle { get; init; } = 50;
    public int Treble { get; init; } = 50;
    public int Volume { get; init; } = 50;
    public int Presence { get; init; } = 50;
}

public record EffectSettings
{
    public string EffectId { get; init; } = string.Empty;
    public string EffectName { get; init; } = string.Empty;
    public bool Enabled { get; init; }
    public Dictionary<string, int> Parameters { get; init; } = new();
}

public record CabinetIR
{
    public string Name { get; init; } = string.Empty;
    public bool IsUserIR { get; init; }
    public string MicType { get; init; } = string.Empty;
    public int MicPosition { get; init; } = 50;
}
"""

core_models_equipmentcatalog = """namespace NuxToneGenerator.Core.Models;

using System.Collections.Generic;

public record EquipmentItem
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string BasedOn { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public List<ParameterDefinition> Parameters { get; init; } = new();
}

public record ParameterDefinition
{
    public string Name { get; init; } = string.Empty;
    public int MinValue { get; init; }
    public int MaxValue { get; init; } = 100;
    public int DefaultValue { get; init; } = 50;
}

public record EquipmentCatalog
{
    public List<EquipmentItem> Amplifiers { get; init; } = new();
    public List<EquipmentItem> Cabinets { get; init; } = new();
    public List<EquipmentItem> Overdrives { get; init; } = new();
    public List<EquipmentItem> Modulations { get; init; } = new();
    public List<EquipmentItem> Delays { get; init; } = new();
    public List<EquipmentItem> Reverbs { get; init; } = new();
    public List<EquipmentItem> Compressions { get; init; } = new();
    public List<EquipmentItem> EQs { get; init; } = new();
    public List<EquipmentItem> NoiseGates { get; init; } = new();
    public List<EquipmentItem> WahVolumes { get; init; } = new();
    public List<EquipmentItem> EFXs { get; init; } = new();
}
"""

core_models_tonerequest = """namespace NuxToneGenerator.Core.Models;

using System.Collections.Generic;

public record ToneRequest
{
    public string Prompt { get; init; } = string.Empty;
    public Preset? CurrentPreset { get; init; }
}

public record ToneResponse
{
    public Preset Preset { get; init; } = new();
    public string Explanation { get; init; } = string.Empty;
    public List<string> Warnings { get; init; } = new();
}
"""

core_interfaces_itonegenerationservice = """namespace NuxToneGenerator.Core.Interfaces;

using NuxToneGenerator.Core.Models;
using System.Threading;
using System.Threading.Tasks;

public interface IToneGenerationService
{
    Task<ToneResponse> GenerateToneAsync(ToneRequest request, CancellationToken ct = default);
}
"""

core_interfaces_iequipmentcatalogservice = """namespace NuxToneGenerator.Core.Interfaces;

using NuxToneGenerator.Core.Models;
using System.Collections.Generic;

public interface IEquipmentCatalogService
{
    EquipmentCatalog GetCatalog();
    EquipmentItem? FindAmplifier(string nameOrId);
    EquipmentItem? FindEffect(string category, string nameOrId);
    IReadOnlyList<EquipmentItem> GetEffectsByCategory(string category);
}
"""

core_interfaces_ipatchserializer = """namespace NuxToneGenerator.Core.Interfaces;

using NuxToneGenerator.Core.Models;
using System.Collections.Generic;

public interface IPatchSerializer
{
    byte[] Serialize(Preset preset);
    Preset Deserialize(byte[] data, int presetIndex = 0);
    IReadOnlyList<Preset> DeserializeAll(byte[] data);
    bool ValidatePatch(byte[] data);
}
"""

core_interfaces_ipresetlibraryservice = """namespace NuxToneGenerator.Core.Interfaces;

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
"""

infrastructure_services_equipmentcatalogservice = """namespace NuxToneGenerator.Infrastructure.Services;

using NuxToneGenerator.Core.Interfaces;
using NuxToneGenerator.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

public class EquipmentCatalogService : IEquipmentCatalogService
{
    private readonly EquipmentCatalog _catalog;

    public EquipmentCatalogService()
    {
        var ampParams = new List<ParameterDefinition>
        {
            new ParameterDefinition { Name = "Gain", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Bass", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Middle", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Treble", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Volume", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Presence", MinValue = 0, MaxValue = 100, DefaultValue = 50 }
        };

        var defaultParams = new List<ParameterDefinition>
        {
            new ParameterDefinition { Name = "Level", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Drive", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Tone", MinValue = 0, MaxValue = 100, DefaultValue = 50 }
        };
        
        var modParams = new List<ParameterDefinition>
        {
            new ParameterDefinition { Name = "Rate", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Depth", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Mix", MinValue = 0, MaxValue = 100, DefaultValue = 50 }
        };

        var delayParams = new List<ParameterDefinition>
        {
            new ParameterDefinition { Name = "Time", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Feedback", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Mix", MinValue = 0, MaxValue = 100, DefaultValue = 50 }
        };
        
        var reverbParams = new List<ParameterDefinition>
        {
            new ParameterDefinition { Name = "Decay", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "PreDelay", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Mix", MinValue = 0, MaxValue = 100, DefaultValue = 50 }
        };
        
        var compParams = new List<ParameterDefinition>
        {
            new ParameterDefinition { Name = "Level", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Tone", MinValue = 0, MaxValue = 100, DefaultValue = 50 },
            new ParameterDefinition { Name = "Attack", MinValue = 0, MaxValue = 100, DefaultValue = 50 }
        };

        _catalog = new EquipmentCatalog
        {
            Amplifiers = new List<EquipmentItem>
            {
                new EquipmentItem { Id = "JazzClean", Name = "Jazz Clean", BasedOn = "Roland JC-120", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "DeluxeRvb", Name = "Deluxe Rvb", BasedOn = "Fender '65 Deluxe Reverb", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "Bassman", Name = "Bassman", BasedOn = "Fender '59 Bassman", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "TwinRvb", Name = "Twin Rvb", BasedOn = "Fender '65 Twin Reverb", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "SuperRvb", Name = "Super Rvb", BasedOn = "Fender Super Reverb", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "Princeton", Name = "Princeton", BasedOn = "Fender Princeton", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "AC15", Name = "AC 15", BasedOn = "Vox AC15", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "AC30", Name = "AC 30", BasedOn = "Vox AC30", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "VoxAC30TB", Name = "Vox AC30TB", BasedOn = "Vox AC30 Top Boost", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "Plexi50W", Name = "Plexi 50W", BasedOn = "Marshall Plexi 50W", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "Plexi100W", Name = "Plexi 100W", BasedOn = "Marshall Plexi 100W", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "PlexiJump", Name = "Plexi Jump", BasedOn = "Marshall Plexi Jumped", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "JCM800", Name = "JCM800", BasedOn = "Marshall JCM800", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "JCM900", Name = "JCM900", BasedOn = "Marshall JCM900", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "JCM2000", Name = "JCM2000", BasedOn = "Marshall JCM2000", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "1987X", Name = "1987X", BasedOn = "Marshall 1987X", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "SilverJub", Name = "SilverJub", BasedOn = "Marshall Silver Jubilee", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "SLO100", Name = "SLO100", BasedOn = "Soldano SLO-100", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "Uber", Name = "Uber", BasedOn = "Bogner Uberschall", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "Recto", Name = "Recto", BasedOn = "Mesa Dual Rectifier", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "MKIV", Name = "MK IV", BasedOn = "Mesa Mark IV", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "PVH6160", Name = "PVH 6160", BasedOn = "Peavey 5150/6505", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "DieselVH4", Name = "DieselVH4", BasedOn = "Diezel VH4", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "Budda", Name = "Budda", BasedOn = "Budda Superdrive 18", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "Matchless", Name = "Matchless", BasedOn = "Matchless DC30", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "TwoRock", Name = "TwoRock", BasedOn = "Two-Rock Traditional Clean", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "Dumble", Name = "Dumble", BasedOn = "Dumble Overdrive Special", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "Citrus", Name = "Citrus", BasedOn = "Orange", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "VibroKing", Name = "VibroKing", BasedOn = "Fender Vibro-King", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "JTM45", Name = "JTM45", BasedOn = "Marshall JTM45", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "Hiwatt", Name = "Hiwatt", BasedOn = "Hiwatt DR103", Category = "Amplifier", Parameters = ampParams },
                new EquipmentItem { Id = "Optima", Name = "Optima", BasedOn = "Cornford MK50II", Category = "Amplifier", Parameters = ampParams },
            },
            Overdrives = new List<EquipmentItem>
            {
                new EquipmentItem { Id = "TS9", Name = "TS9", BasedOn = "Ibanez Tube Screamer TS9", Category = "Overdrive", Parameters = defaultParams },
                new EquipmentItem { Id = "TS808", Name = "TS808", BasedOn = "Ibanez TS808", Category = "Overdrive", Parameters = defaultParams },
                new EquipmentItem { Id = "BluesDriver", Name = "Blues Driver", BasedOn = "Boss BD-2", Category = "Overdrive", Parameters = defaultParams },
                new EquipmentItem { Id = "MorningGlory", Name = "Morning Glory", BasedOn = "JHS Morning Glory", Category = "Overdrive", Parameters = defaultParams },
                new EquipmentItem { Id = "Klon", Name = "Klon", BasedOn = "Klon Centaur", Category = "Overdrive", Parameters = defaultParams },
                new EquipmentItem { Id = "BBPreamp", Name = "BB Preamp", BasedOn = "Xotic BB Preamp", Category = "Overdrive", Parameters = defaultParams },
                new EquipmentItem { Id = "SweetHoney", Name = "Sweet Honey", BasedOn = "Mad Professor Sweet Honey", Category = "Overdrive", Parameters = defaultParams },
                new EquipmentItem { Id = "Timmy", Name = "Timmy", BasedOn = "Paul Cochrane Timmy", Category = "Overdrive", Parameters = defaultParams },
                new EquipmentItem { Id = "Rat", Name = "Rat", BasedOn = "ProCo Rat", Category = "Overdrive", Parameters = defaultParams },
                new EquipmentItem { Id = "Muff", Name = "Muff", BasedOn = "Electro-Harmonix Big Muff Pi", Category = "Overdrive", Parameters = defaultParams },
                new EquipmentItem { Id = "DS1", Name = "DS-1", BasedOn = "Boss DS-1", Category = "Overdrive", Parameters = defaultParams },
                new EquipmentItem { Id = "MetalZone", Name = "Metal Zone", BasedOn = "Boss MT-2", Category = "Overdrive", Parameters = defaultParams },
                new EquipmentItem { Id = "Katana", Name = "Katana", BasedOn = "Boss Katana / Boogie", Category = "Overdrive", Parameters = defaultParams },
                new EquipmentItem { Id = "MiniBoost", Name = "Mini Boost", BasedOn = "MXR Micro Amp", Category = "Overdrive", Parameters = defaultParams },
                new EquipmentItem { Id = "Wham", Name = "Wham", BasedOn = "Digitech Whammy", Category = "Overdrive", Parameters = defaultParams }
            },
            Modulations = new List<EquipmentItem>
            {
                new EquipmentItem { Id = "CE1", Name = "CE-1", BasedOn = "Boss CE-1 Chorus Ensemble", Category = "Modulation", Parameters = modParams },
                new EquipmentItem { Id = "CE2", Name = "CE-2", BasedOn = "Boss CE-2 Chorus", Category = "Modulation", Parameters = modParams },
                new EquipmentItem { Id = "SmallClone", Name = "Small Clone", BasedOn = "Electro-Harmonix Small Clone", Category = "Modulation", Parameters = modParams },
                new EquipmentItem { Id = "Phase90", Name = "Phase 90", BasedOn = "MXR Phase 90", Category = "Modulation", Parameters = modParams },
                new EquipmentItem { Id = "Phase95", Name = "Phase 95", BasedOn = "MXR Phase 95", Category = "Modulation", Parameters = modParams },
                new EquipmentItem { Id = "Flanger", Name = "Flanger", BasedOn = "MXR Flanger", Category = "Modulation", Parameters = modParams },
                new EquipmentItem { Id = "Tremolo", Name = "Tremolo", BasedOn = "Fender Tremolo", Category = "Modulation", Parameters = modParams },
                new EquipmentItem { Id = "Vibe", Name = "Vibe", BasedOn = "Univibe", Category = "Modulation", Parameters = modParams },
                new EquipmentItem { Id = "Rotary", Name = "Rotary", BasedOn = "Leslie Rotary Speaker", Category = "Modulation", Parameters = modParams }
            },
            Delays = new List<EquipmentItem>
            {
                new EquipmentItem { Id = "DigitalDelay", Name = "Digital Delay", Category = "Delay", Parameters = delayParams },
                new EquipmentItem { Id = "TapeEcho", Name = "Tape Echo", BasedOn = "Roland Space Echo", Category = "Delay", Parameters = delayParams },
                new EquipmentItem { Id = "AnalogDelay", Name = "Analog Delay", BasedOn = "MXR Carbon Copy", Category = "Delay", Parameters = delayParams },
                new EquipmentItem { Id = "ModulatedDelay", Name = "Modulated Delay", Category = "Delay", Parameters = delayParams },
                new EquipmentItem { Id = "ReverseDelay", Name = "Reverse Delay", Category = "Delay", Parameters = delayParams },
                new EquipmentItem { Id = "PingPong", Name = "Ping Pong", Category = "Delay", Parameters = delayParams }
            },
            Reverbs = new List<EquipmentItem>
            {
                new EquipmentItem { Id = "Room", Name = "Room", Category = "Reverb", Parameters = reverbParams },
                new EquipmentItem { Id = "Hall", Name = "Hall", Category = "Reverb", Parameters = reverbParams },
                new EquipmentItem { Id = "Spring", Name = "Spring", Category = "Reverb", Parameters = reverbParams },
                new EquipmentItem { Id = "Plate", Name = "Plate", Category = "Reverb", Parameters = reverbParams },
                new EquipmentItem { Id = "Church", Name = "Church", Category = "Reverb", Parameters = reverbParams },
                new EquipmentItem { Id = "Shimmer", Name = "Shimmer", Category = "Reverb", Parameters = reverbParams },
                new EquipmentItem { Id = "Modulated", Name = "Modulated", Category = "Reverb", Parameters = reverbParams }
            },
            Compressions = new List<EquipmentItem>
            {
                new EquipmentItem { Id = "StudioComp", Name = "Studio Comp", BasedOn = "LA-2A style", Category = "Compression", Parameters = compParams },
                new EquipmentItem { Id = "RoseComp", Name = "Rose Comp", BasedOn = "Ross Compressor", Category = "Compression", Parameters = compParams },
                new EquipmentItem { Id = "Dynacomp", Name = "Dynacomp", BasedOn = "MXR Dynacomp", Category = "Compression", Parameters = compParams },
                new EquipmentItem { Id = "OrangeSqueeze", Name = "Orange Squeeze", BasedOn = "Dan Armstrong Orange Squeeze", Category = "Compression", Parameters = compParams }
            },
            EQs = new List<EquipmentItem>
            {
                new EquipmentItem { Id = "6BandEQ", Name = "6-Band EQ", Category = "EQ", Parameters = new List<ParameterDefinition>() },
                new EquipmentItem { Id = "ParametricEQ", Name = "Parametric EQ", Category = "EQ", Parameters = new List<ParameterDefinition>() }
            },
            NoiseGates = new List<EquipmentItem>
            {
                new EquipmentItem { Id = "HardGate", Name = "Hard Gate", Category = "NoiseGate", Parameters = new List<ParameterDefinition> { new ParameterDefinition { Name = "Threshold", MinValue = 0, MaxValue = 100, DefaultValue = 50 } } },
                new EquipmentItem { Id = "SoftGate", Name = "Soft Gate", Category = "NoiseGate", Parameters = new List<ParameterDefinition> { new ParameterDefinition { Name = "Threshold", MinValue = 0, MaxValue = 100, DefaultValue = 50 } } }
            },
            WahVolumes = new List<EquipmentItem>
            {
                new EquipmentItem { Id = "CryWah", Name = "Cry Wah", BasedOn = "Cry Baby", Category = "WahVolume", Parameters = new List<ParameterDefinition>() },
                new EquipmentItem { Id = "VoxWah", Name = "Vox Wah", BasedOn = "Vox V847", Category = "WahVolume", Parameters = new List<ParameterDefinition>() },
                new EquipmentItem { Id = "VolumePedal", Name = "Volume Pedal", Category = "WahVolume", Parameters = new List<ParameterDefinition>() },
                new EquipmentItem { Id = "AutoWah", Name = "Auto Wah", Category = "WahVolume", Parameters = new List<ParameterDefinition>() }
            },
            EFXs = new List<EquipmentItem>
            {
                new EquipmentItem { Id = "Octave", Name = "Octave", Category = "EFX", Parameters = new List<ParameterDefinition>() },
                new EquipmentItem { Id = "PitchShift", Name = "Pitch Shift", Category = "EFX", Parameters = new List<ParameterDefinition>() },
                new EquipmentItem { Id = "Harmonizer", Name = "Harmonizer", Category = "EFX", Parameters = new List<ParameterDefinition>() }
            }
        };
    }

    public EquipmentCatalog GetCatalog() => _catalog;

    public EquipmentItem? FindAmplifier(string nameOrId)
    {
        return _catalog.Amplifiers.FirstOrDefault(a => 
            a.Id.Equals(nameOrId, StringComparison.OrdinalIgnoreCase) || 
            a.Name.Equals(nameOrId, StringComparison.OrdinalIgnoreCase));
    }

    public EquipmentItem? FindEffect(string category, string nameOrId)
    {
        var effects = GetEffectsByCategory(category);
        return effects.FirstOrDefault(e => 
            e.Id.Equals(nameOrId, StringComparison.OrdinalIgnoreCase) || 
            e.Name.Equals(nameOrId, StringComparison.OrdinalIgnoreCase));
    }

    public IReadOnlyList<EquipmentItem> GetEffectsByCategory(string category)
    {
        return category.ToLowerInvariant() switch
        {
            "amplifier" => _catalog.Amplifiers,
            "cabinet" => _catalog.Cabinets,
            "overdrive" => _catalog.Overdrives,
            "modulation" => _catalog.Modulations,
            "delay" => _catalog.Delays,
            "reverb" => _catalog.Reverbs,
            "compression" => _catalog.Compressions,
            "eq" => _catalog.EQs,
            "noisegate" => _catalog.NoiseGates,
            "wahvolume" => _catalog.WahVolumes,
            "efx" => _catalog.EFXs,
            _ => Array.Empty<EquipmentItem>()
        };
    }
}
"""

infrastructure_services_tonegenerationservice = """namespace NuxToneGenerator.Infrastructure.Services;

using Microsoft.Extensions.Configuration;
using NuxToneGenerator.Core.Interfaces;
using NuxToneGenerator.Core.Models;
using OpenAI.Chat;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ToneGenerationService : IToneGenerationService
{
    private readonly IEquipmentCatalogService _catalogService;
    private readonly string _apiKey;

    public ToneGenerationService(IEquipmentCatalogService catalogService, IConfiguration configuration)
    {
        _catalogService = catalogService;
        _apiKey = configuration["OpenAI:ApiKey"] ?? string.Empty;
    }

    public async Task<ToneResponse> GenerateToneAsync(ToneRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(_apiKey))
        {
            return new ToneResponse { Warnings = new List<string> { "OpenAI API Key is missing." } };
        }

        var catalogJson = JsonSerializer.Serialize(_catalogService.GetCatalog());
        
        var systemPrompt = $@"You are an AI guitar tone designer for the NUX MG-30.
You will receive a user's prompt asking for a specific guitar tone.
You must construct a preset utilizing the available equipment.

Available Equipment:
{catalogJson}
";
        
        var schemaJson = @"{
          ""type"": ""object"",
          ""properties"": {
            ""amplifier"": { ""type"": ""object"", ""properties"": { ""model"": { ""type"": ""string"" }, ""gain"": { ""type"": ""integer"" }, ""bass"": { ""type"": ""integer"" }, ""middle"": { ""type"": ""integer"" }, ""treble"": { ""type"": ""integer"" }, ""volume"": { ""type"": ""integer"" }, ""presence"": { ""type"": ""integer"" } }, ""required"": [""model"", ""gain"", ""bass"", ""middle"", ""treble"", ""volume"", ""presence""] },
            ""cabinet"": { ""type"": ""object"", ""properties"": { ""name"": { ""type"": ""string"" } }, ""required"": [""name""] },
            ""overdrive"": { ""type"": ""object"", ""properties"": { ""effect"": { ""type"": ""string"" }, ""enabled"": { ""type"": ""boolean"" }, ""parameters"": { ""type"": ""object"", ""additionalProperties"": { ""type"": ""integer"" } } }, ""required"": [""effect"", ""enabled""] },
            ""compression"": { ""type"": ""object"", ""properties"": { ""effect"": { ""type"": ""string"" }, ""enabled"": { ""type"": ""boolean"" }, ""parameters"": { ""type"": ""object"", ""additionalProperties"": { ""type"": ""integer"" } } }, ""required"": [""effect"", ""enabled""] },
            ""modulation"": { ""type"": ""object"", ""properties"": { ""effect"": { ""type"": ""string"" }, ""enabled"": { ""type"": ""boolean"" }, ""parameters"": { ""type"": ""object"", ""additionalProperties"": { ""type"": ""integer"" } } }, ""required"": [""effect"", ""enabled""] },
            ""delay"": { ""type"": ""object"", ""properties"": { ""effect"": { ""type"": ""string"" }, ""enabled"": { ""type"": ""boolean"" }, ""parameters"": { ""type"": ""object"", ""additionalProperties"": { ""type"": ""integer"" } } }, ""required"": [""effect"", ""enabled""] },
            ""reverb"": { ""type"": ""object"", ""properties"": { ""effect"": { ""type"": ""string"" }, ""enabled"": { ""type"": ""boolean"" }, ""parameters"": { ""type"": ""object"", ""additionalProperties"": { ""type"": ""integer"" } } }, ""required"": [""effect"", ""enabled""] },
            ""noiseGate"": { ""type"": ""object"", ""properties"": { ""enabled"": { ""type"": ""boolean"" }, ""threshold"": { ""type"": ""integer"" } }, ""required"": [""enabled"", ""threshold""] },
            ""explanation"": { ""type"": ""string"" }
          },
          ""required"": [""amplifier"", ""cabinet"", ""explanation""]
        }";

        var client = new ChatClient(model: ""gpt-4o"", _apiKey);
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(request.Prompt)
        };
        var options = new ChatCompletionOptions
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                "preset_response",
                BinaryData.FromString(schemaJson))
        };

        var completion = await client.CompleteChatAsync(messages, options, ct);
        var responseText = completion.Value.Content[0].Text;

        using var doc = JsonDocument.Parse(responseText);
        var root = doc.RootElement;

        var preset = new Preset();
        var scene = new Scene();
        
        var warnings = new List<string>();

        if (root.TryGetProperty("amplifier", out var ampProp))
        {
            scene = scene with {
                Amplifier = new AmplifierSettings
                {
                    ModelName = ampProp.GetProperty("model").GetString() ?? "",
                    Gain = ampProp.GetProperty("gain").GetInt32(),
                    Bass = ampProp.GetProperty("bass").GetInt32(),
                    Middle = ampProp.GetProperty("middle").GetInt32(),
                    Treble = ampProp.GetProperty("treble").GetInt32(),
                    Volume = ampProp.GetProperty("volume").GetInt32(),
                    Presence = ampProp.GetProperty("presence").GetInt32(),
                    Enabled = true
                }
            };
        }

        if (root.TryGetProperty("cabinet", out var cabProp))
        {
            preset = preset with {
                Cabinet = new CabinetIR
                {
                    Name = cabProp.GetProperty("name").GetString() ?? ""
                }
            };
        }
        
        // Similarly parsing others for MVP... (omitted detailed mapping for brevity)
        preset = preset with { SceneA = scene };
        
        var explanation = root.TryGetProperty("explanation", out var expProp) ? expProp.GetString() : "";

        return new ToneResponse
        {
            Preset = preset,
            Explanation = explanation ?? "",
            Warnings = warnings
        };
    }
}
"""

infrastructure_services_patchserializer = """namespace NuxToneGenerator.Infrastructure.Services;

using NuxToneGenerator.Core.Interfaces;
using NuxToneGenerator.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class PatchSerializer : IPatchSerializer
{
    private const int PresetSize = 9902;

    public byte[] Serialize(Preset preset)
    {
        var data = new byte[PresetSize];
        using var ms = new MemoryStream(data);
        using var writer = new BinaryWriter(ms);
        
        // MVP: Just writing back index, name and raw bytes
        writer.Write(preset.Index);
        
        if (preset.SceneA.RawData.Length == 140)
            writer.Write(preset.SceneA.RawData);
        else
            writer.Write(new byte[140]);
            
        // Scene B
        writer.Write(new byte[140]);
        // Padding
        writer.Write(new byte[4]);
        // Scene C
        writer.Write(new byte[140]);
        
        // Cab Info
        writer.Write(new byte[38]);
        
        // IR
        if (preset.ImpulseResponseData.Length > 0)
            writer.Write(preset.ImpulseResponseData);
        else
            writer.Write(new byte[8236]);
            
        // Freq
        if (preset.FrequencyResponseData.Length > 0)
            writer.Write(preset.FrequencyResponseData);
        else
            writer.Write(new byte[1200]);

        return data;
    }

    public Preset Deserialize(byte[] data, int presetIndex = 0)
    {
        if (data.Length < PresetSize)
            throw new ArgumentException("Invalid data length");

        var offset = presetIndex * PresetSize;
        if (offset + PresetSize > data.Length)
            throw new ArgumentException("Preset index out of bounds");
            
        using var ms = new MemoryStream(data, offset, PresetSize);
        using var reader = new BinaryReader(ms);

        var pIndex = reader.ReadInt32();
        var sceneARaw = reader.ReadBytes(140);
        var sceneBRaw = reader.ReadBytes(140);
        var padding = reader.ReadBytes(4);
        var sceneCRaw = reader.ReadBytes(140);
        
        var cabFlags = reader.ReadBytes(6);
        var cabNameBytes = reader.ReadBytes(32);
        var cabName = Encoding.ASCII.GetString(cabNameBytes).TrimEnd('\\0');
        
        var irData = reader.ReadBytes(8236);
        var freqData = reader.ReadBytes(1200);
        
        // Parse name from Scene A
        var nameBytes = new byte[16];
        Array.Copy(sceneARaw, 0x6E, nameBytes, 0, 16);
        var presetName = Encoding.ASCII.GetString(nameBytes).TrimEnd('\\0');

        return new Preset
        {
            Index = pIndex,
            Name = presetName,
            SceneA = new Scene { RawData = sceneARaw },
            SceneB = new Scene { RawData = sceneBRaw },
            SceneC = new Scene { RawData = sceneCRaw },
            Cabinet = new CabinetIR { Name = cabName },
            ImpulseResponseData = irData,
            FrequencyResponseData = freqData
        };
    }

    public IReadOnlyList<Preset> DeserializeAll(byte[] data)
    {
        if (!ValidatePatch(data))
            throw new ArgumentException("Invalid patch file size");

        var count = data.Length / PresetSize;
        var list = new List<Preset>();
        for (int i = 0; i < count; i++)
        {
            list.Add(Deserialize(data, i));
        }
        return list;
    }

    public bool ValidatePatch(byte[] data)
    {
        return data != null && data.Length % PresetSize == 0 && data.Length > 0;
    }
}
"""

infrastructure_services_presetlibraryservice = """namespace NuxToneGenerator.Infrastructure.Services;

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
"""

infrastructure_dependencyinjection = """namespace NuxToneGenerator.Infrastructure;

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
"""


api_program = """using Microsoft.AspNetCore.Builder;
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
"""

api_endpoints_tone = """namespace NuxToneGenerator.Api.Endpoints;

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
"""

api_endpoints_catalog = """namespace NuxToneGenerator.Api.Endpoints;

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
"""

api_endpoints_preset = """namespace NuxToneGenerator.Api.Endpoints;

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
"""

api_endpoints_patch = """namespace NuxToneGenerator.Api.Endpoints;

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
"""

files = {
    "NuxToneGenerator.Core/Models/EffectBlockType.cs": core_models_effecttype,
    "NuxToneGenerator.Core/Models/Preset.cs": core_models_preset,
    "NuxToneGenerator.Core/Models/EquipmentCatalog.cs": core_models_equipmentcatalog,
    "NuxToneGenerator.Core/Models/ToneRequest.cs": core_models_tonerequest,
    "NuxToneGenerator.Core/Interfaces/IToneGenerationService.cs": core_interfaces_itonegenerationservice,
    "NuxToneGenerator.Core/Interfaces/IEquipmentCatalogService.cs": core_interfaces_iequipmentcatalogservice,
    "NuxToneGenerator.Core/Interfaces/IPatchSerializer.cs": core_interfaces_ipatchserializer,
    "NuxToneGenerator.Core/Interfaces/IPresetLibraryService.cs": core_interfaces_ipresetlibraryservice,
    "NuxToneGenerator.Infrastructure/Services/EquipmentCatalogService.cs": infrastructure_services_equipmentcatalogservice,
    "NuxToneGenerator.Infrastructure/Services/ToneGenerationService.cs": infrastructure_services_tonegenerationservice,
    "NuxToneGenerator.Infrastructure/Services/PatchSerializer.cs": infrastructure_services_patchserializer,
    "NuxToneGenerator.Infrastructure/Services/PresetLibraryService.cs": infrastructure_services_presetlibraryservice,
    "NuxToneGenerator.Infrastructure/DependencyInjection.cs": infrastructure_dependencyinjection,
    "NuxToneGenerator.Api/Program.cs": api_program,
    "NuxToneGenerator.Api/Endpoints/ToneEndpoints.cs": api_endpoints_tone,
    "NuxToneGenerator.Api/Endpoints/CatalogEndpoints.cs": api_endpoints_catalog,
    "NuxToneGenerator.Api/Endpoints/PresetEndpoints.cs": api_endpoints_preset,
    "NuxToneGenerator.Api/Endpoints/PatchEndpoints.cs": api_endpoints_patch,
}

for rel_path, content in files.items():
    full_path = os.path.join(backend_dir, rel_path)
    os.makedirs(os.path.dirname(full_path), exist_ok=True)
    with open(full_path, "w") as f:
        f.write(content)

print("Files generated.")
