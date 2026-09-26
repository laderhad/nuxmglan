namespace NuxToneGenerator.Core.Models;

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
