namespace NuxToneGenerator.Core.Models;

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
